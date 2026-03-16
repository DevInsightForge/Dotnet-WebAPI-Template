using DevInsightForge.Application.Abstructions;
using DevInsightForge.Application.Abstructions.DataAccess;
using DevInsightForge.Application.DtoModels.User;
using DevInsightForge.Application.Results;
using DevInsightForge.Domain.Entities;

namespace DevInsightForge.Application.Features.Users.Commands;

public sealed record CreateUserCommand(CreateUserRequestDto Dto) : IRequest<CreateUserCommand, Task<Result<UserResponseModel>>>;

internal sealed class CreateUserCommandHandler(
    IUnitOfWork unitOfWork,
    IEncryptionService encryptionService,
    IValidator<CreateUserRequestDto> createUserValidator) : IRequestHandler<CreateUserCommand, Task<Result<UserResponseModel>>>
{
    public async Task<Result<UserResponseModel>> Handle(CreateUserCommand request, CancellationToken ct)
    {
        var validationResult = await createUserValidator.ValidateAsync(request.Dto, ct);
        if (!validationResult.IsValid)
        {
            return Result<UserResponseModel>.ValidationFailure(validationResult);
        }

        var user = UserModel.CreateUser(request.Dto.Email.Trim())
            .SetPasswordHash(encryptionService.HashPassword(request.Dto.Password));

        if (request.Dto.IsEmailVerified)
        {
            user.MarkEmailAsVerified();
        }

        await unitOfWork.WithTransaction(async innerCt =>
        {
            await unitOfWork.Users.AddAsync(user, innerCt);
            await unitOfWork.SaveChangesAsync(innerCt);
        }, ct);

        return Result<UserResponseModel>.Created(user.Adapt<UserResponseModel>());
    }
}
