using DevInsightForge.Application.DtoModels.Authentication;
using DevInsightForge.Application.Abstructions;
using DevInsightForge.Application.Abstructions.DataAccess;
using DevInsightForge.Application.Results;
using System.Security.Claims;

namespace DevInsightForge.Application.Features.Authentication.Commands;

public sealed record AuthenticateUserCommand(AuthenticateUserDto Dto) : IRequest<AuthenticateUserCommand, Task<Result<TokenResponseModel>>>;

internal sealed class AuthenticateUserCommandHandler(
    IEncryptionService encryptionService,
    IEmailService emailService,
    IUnitOfWork unitOfWork,
    ITokenService tokenServices,
    IValidator<AuthenticateUserDto> authenticateUserDtoValidator) : IRequestHandler<AuthenticateUserCommand, Task<Result<TokenResponseModel>>>
{
    public async Task<Result<TokenResponseModel>> Handle(AuthenticateUserCommand request, CancellationToken ct)
    {
        var validationResult = await authenticateUserDtoValidator.ValidateAsync(request.Dto, ct);
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
        }, ct);

        await SendLoginAlertEmailAsync(emailService, user.Email, ct);

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

    private static async Task SendLoginAlertEmailAsync(
        IEmailService emailService,
        string userEmail,
        CancellationToken ct)
    {
        var loginDateTime = DateTime.UtcNow.ToString("f");
        var emailBody = $"""
            <body>
                <main>
                    <section>
                        <p>Dear User,</p>
                        <p>We detected a sign-in to your <strong>DevInsightForge</strong> account on <strong>{loginDateTime} UTC</strong>.</p>
                        <p>If this was not you, please reset your password immediately.</p>
                    </section>
                </main>
                <footer>
                    <p>This is an automated message. Please do not reply.</p>
                </footer>
            </body>
            """;

        try
        {
            await emailService.SendAsync(
                to: userEmail,
                subject: "Login Alert - DevInsightForge",
                body: emailBody,
                isHtml: true,
                ct: ct);
        }
        catch
        {
            // Email delivery should not block authentication success.
        }
    }
}




