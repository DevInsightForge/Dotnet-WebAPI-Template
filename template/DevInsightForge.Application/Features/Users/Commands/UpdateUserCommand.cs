using DevInsightForge.Application.Abstractions.DataAccess;
using DevInsightForge.Application.Abstractions.InternalServices;
using DevInsightForge.Application.Contracts.User;
using DevInsightForge.Application.Results;

namespace DevInsightForge.Application.Features.Users.Commands;

public sealed record UpdateUserCommand(Guid UserId, UpdateUserRequestDto Request) : IRequest<UpdateUserCommand, Task<Result<UserResponseDto>>>;

internal sealed class UpdateUserCommandHandler(
    IUnitOfWork unitOfWork,
    IEncryptionService encryptionService,
    IValidator<UpdateUserRequestDto> updateUserValidator) : IRequestHandler<UpdateUserCommand, Task<Result<UserResponseDto>>>
{
    public async Task<Result<UserResponseDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await updateUserValidator.ValidateAsync(request.Request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<UserResponseDto>.ValidationFailure(validationResult);
        }

        var user = await unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user is null)
        {
            return Result<UserResponseDto>.Failure(
                new Error("user.not_found", "User not found.", ErrorType.NotFound));
        }

        var normalizedEmail = request.Request.Email.Trim().ToLowerInvariant();
        var emailInUse = await unitOfWork.Users.AnyAsync(u => u.Email == normalizedEmail && u.Id != user.Id);
        if (emailInUse)
        {
            return Result<UserResponseDto>.Failure(
                new Error("user.email_conflict", "Email is already registered.", ErrorType.Conflict));
        }

        user.SetEmail(request.Request.Email);
        user.SetRoleId(request.Request.RoleId);

        if (!string.IsNullOrWhiteSpace(request.Request.Password))
        {
            user.SetPasswordHash(encryptionService.HashPassword(request.Request.Password));
        }

        await unitOfWork.WithTransaction(async innerCt =>
        {
            await unitOfWork.Users.UpdateAsync(user, innerCt);
            await unitOfWork.SaveChangesAsync(innerCt);
        }, cancellationToken);

        var updatedUser = await unitOfWork.Users.GetWhereAsync(u => u.Id == user.Id, u => u.Role!);
        if (updatedUser is null)
        {
            return Result<UserResponseDto>.Failure(
                new Error("user.not_found", "User not found.", ErrorType.NotFound));
        }

        return Result<UserResponseDto>.Success(updatedUser.Adapt<UserResponseDto>());
    }
}



