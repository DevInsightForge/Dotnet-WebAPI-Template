using DevInsightForge.Application.DtoModels.Authentication;
using DevInsightForge.Application.Abstructions;
using DevInsightForge.Application.Abstructions.DataAccess;
using DevInsightForge.Application.Results;
using System.Security.Claims;

namespace DevInsightForge.Application.Features.Authentication.Commands;

public sealed record AuthenticateUserCommand(AuthenticateUserDto Dto) : IRequest<AuthenticateUserCommand, Task<Result<TokenResponseModel>>>;

internal sealed class AuthenticateUserCommandHandler(
    IEncryptionService encryptionService,
    IUnitOfWork unitOfWork,
    ITokenService tokenServices,
    IValidator<AuthenticateUserDto> authenticateUserDtoValidator) : IRequestHandler<AuthenticateUserCommand, Task<Result<TokenResponseModel>>>
{
    public async Task<Result<TokenResponseModel>> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await authenticateUserDtoValidator.ValidateAsync(request.Dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<TokenResponseModel>.ValidationFailure(validationResult);
        }

        var normalizedEmail = request.Dto.Email.ToUpperInvariant();
        var user = await unitOfWork.Users.GetWhereAsync(u => u.NormalizedEmail.Equals(normalizedEmail));

        if (user is null || !encryptionService.VerifyPassword(user.PasswordHash, request.Dto.Password))
        {
            return Result<TokenResponseModel>.Failure(
                new Error("auth.invalid_credentials", "Invalid credentials.", ErrorType.Unauthorized));
        }

        user.UpdateLastLogin();
        await unitOfWork.WithTransaction(async ct =>
        {
            await unitOfWork.Users.UpdateAsync(user, ct);
            await unitOfWork.SaveChangesAsync(ct);
        }, cancellationToken);

        var claims = new List<Claim>
        {
            new(ClaimTypes.Sid, user.Id.ToString(), ClaimValueTypes.Sid)
        };

        var (accessToken, accessTokenExpiresAt) = tokenServices.GenerateJwtToken(claims);

        return Result<TokenResponseModel>.Success(new TokenResponseModel()
        {
            AccessToken = accessToken,
            AccessTokenExpiresAt = accessTokenExpiresAt
        });
    }
}



