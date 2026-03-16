using DevInsightForge.Application.Abstructions;
using DevInsightForge.Application.Abstructions.DataAccess;
using DevInsightForge.Application.DtoModels.User;
using DevInsightForge.Application.Results;

namespace DevInsightForge.Application.Features.Users.Commands;

public sealed record UpdateUserCommand(Guid UserId, UpdateUserRequestDto Dto) : IRequest<UpdateUserCommand, Task<Result<UserResponseModel>>>;

internal sealed class UpdateUserCommandHandler(
    IUnitOfWork unitOfWork,
    IEncryptionService encryptionService,
    IValidator<UpdateUserRequestDto> updateUserValidator) : IRequestHandler<UpdateUserCommand, Task<Result<UserResponseModel>>>
{
    public async Task<Result<UserResponseModel>> Handle(UpdateUserCommand request, CancellationToken ct)
    {
        var validationResult = await updateUserValidator.ValidateAsync(request.Dto, ct);
        if (!validationResult.IsValid)
        {
            return Result<UserResponseModel>.ValidationFailure(validationResult);
        }

        var user = await unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user is null)
        {
            return Result<UserResponseModel>.Failure(
                new Error("user.not_found", "User not found.", ErrorType.NotFound));
        }

        var normalizedEmail = request.Dto.Email.Trim().ToLowerInvariant();
        var existingWithSameEmail = await unitOfWork.Users.GetWhereAsync(u => u.Email == normalizedEmail);
        if (existingWithSameEmail is not null && existingWithSameEmail.Id != user.Id)
        {
            return Result<UserResponseModel>.Failure(
                new Error("user.email_conflict", "Email is already registered.", ErrorType.Conflict));
        }

        user.SetEmail(request.Dto.Email);

        if (!string.IsNullOrWhiteSpace(request.Dto.Password))
        {
            user.SetPasswordHash(encryptionService.HashPassword(request.Dto.Password));
        }

        if (request.Dto.IsEmailVerified)
        {
            user.MarkEmailAsVerified();
        }

        await unitOfWork.WithTransaction(async innerCt =>
        {
            await unitOfWork.Users.UpdateAsync(user, innerCt);
            await unitOfWork.SaveChangesAsync(innerCt);
        }, ct);

        return Result<UserResponseModel>.Success(user.Adapt<UserResponseModel>());
    }
}
