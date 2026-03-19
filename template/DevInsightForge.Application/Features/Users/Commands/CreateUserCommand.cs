using DevInsightForge.Application.Abstractions.DataAccess;
using DevInsightForge.Application.Abstractions.InternalServices;
using DevInsightForge.Application.Contracts.User;
using DevInsightForge.Application.Results;
using DevInsightForge.Domain.Entities;

namespace DevInsightForge.Application.Features.Users.Commands;

public sealed record CreateUserCommand(CreateUserRequestDto Request) : IRequest<CreateUserCommand, Task<Result<UserResponseDto>>>;

internal sealed class CreateUserCommandHandler(
    IUnitOfWork unitOfWork,
    IEncryptionService encryptionService,
    IValidator<CreateUserRequestDto> createUserValidator) : IRequestHandler<CreateUserCommand, Task<Result<UserResponseDto>>>
{
    public async Task<Result<UserResponseDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await createUserValidator.ValidateAsync(request.Request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<UserResponseDto>.ValidationFailure(validationResult);
        }

        var user = User.Create(request.Request.Email.Trim())
            .SetPasswordHash(encryptionService.HashPassword(request.Request.Password));

        await unitOfWork.WithTransaction(async innerCt =>
        {
            await unitOfWork.Users.AddAsync(user, innerCt);
            await unitOfWork.SaveChangesAsync(innerCt);
        }, cancellationToken);

        return Result<UserResponseDto>.Created(user.Adapt<UserResponseDto>());
    }
}



