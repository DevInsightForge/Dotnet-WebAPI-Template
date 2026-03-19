using DevInsightForge.Application.Abstractions.DataAccess;
using DevInsightForge.Application.Abstractions.InternalServices;
using DevInsightForge.Application.Contracts.Authentication;
using DevInsightForge.Application.Results;
using DevInsightForge.Domain.Entities;

namespace DevInsightForge.Application.Features.Authentication.Commands;

public sealed record RegisterCommand(RegisterRequestDto Request) : IRequest<RegisterCommand, Task<Result>>;

internal sealed class RegisterCommandHandler(
    IEncryptionService encryptionService,
    IUnitOfWork unitOfWork,
    IValidator<RegisterRequestDto> registerValidator) : IRequestHandler<RegisterCommand, Task<Result>>
{
    public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await registerValidator.ValidateAsync(request.Request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.ValidationFailure(validationResult);
        }

        var user = User.Create(request.Request.Email.Trim())
            .SetPasswordHash(encryptionService.HashPassword(request.Request.Password));

        await unitOfWork.WithTransaction(async innerCt =>
        {
            await unitOfWork.Users.AddAsync(user, innerCt);
            await unitOfWork.SaveChangesAsync(innerCt);
        }, cancellationToken);

        return Result.Created();
    }
}



