using System.Security.Claims;
using DevInsightForge.Application.Abstractions.DataAccess;
using DevInsightForge.Application.Abstractions.ExternalServices;
using DevInsightForge.Application.Abstractions.InternalServices;
using DevInsightForge.Application.Contracts.Authentication;
using DevInsightForge.Application.Contracts.Common;
using DevInsightForge.Application.Contracts.User;
using DevInsightForge.Application.Results;

namespace DevInsightForge.Application.Features.Authentication.Commands;

public sealed record LoginCommand(LoginRequestDto Request) : IRequest<LoginCommand, Task<Result<AuthSessionResponseDto>>>;

internal sealed class LoginCommandHandler(
    IEncryptionService encryptionService,
    IEmailService emailService,
    ITokenService tokenService,
    IUnitOfWork unitOfWork,
    IValidator<LoginRequestDto> loginValidator) : IRequestHandler<LoginCommand, Task<Result<AuthSessionResponseDto>>>
{
    public async Task<Result<AuthSessionResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await loginValidator.ValidateAsync(request.Request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<AuthSessionResponseDto>.ValidationFailure(validationResult);
        }

        var normalizedEmail = request.Request.Email.Trim().ToLowerInvariant();
        var user = await unitOfWork.Users.GetWhereAsync(u => u.Email == normalizedEmail);

        if (user is null || !encryptionService.VerifyPassword(user.PasswordHash, request.Request.Password))
        {
            return Result<AuthSessionResponseDto>.Failure(
                new Error("auth.invalid_credentials", "Invalid credentials.", ErrorType.Unauthorized));
        }

        if (!user.IsEmailVerified)
        {
            return Result<AuthSessionResponseDto>.Failure(
                new Error("auth.email_not_verified", "Email address is not verified.", ErrorType.Forbidden));
        }

        user.UpdateLastLogin();
        await unitOfWork.WithTransaction(async innerCt =>
        {
            await unitOfWork.Users.UpdateAsync(user, innerCt);
            await unitOfWork.SaveChangesAsync(innerCt);
        }, cancellationToken);

        await SendLoginAlertEmailAsync(emailService, user.Email, cancellationToken);

        var (token, expiry) = tokenService.GenerateJwtToken(CreateClaims(user.Id, user.Email));

        return Result<AuthSessionResponseDto>.Success(new AuthSessionResponseDto
        {
            AccessToken = token,
            AccessTokenExpiresAt = expiry,
            User = user.Adapt<UserResponseDto>()
        });
    }

    private static List<Claim> CreateClaims(Guid userId, string email) =>
    [
        new(ClaimTypes.Sid, userId.ToString(), ClaimValueTypes.String),
        new(ClaimTypes.NameIdentifier, userId.ToString(), ClaimValueTypes.String),
        new(ClaimTypes.Email, email, ClaimValueTypes.Email)
    ];

    private static async Task SendLoginAlertEmailAsync(
        IEmailService emailService,
        string userEmail,
        CancellationToken cancellationToken)
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
            await emailService.SendAsync(new EmailMessageDto
            {
                To = userEmail,
                Subject = "Login Alert - DevInsightForge",
                Body = emailBody,
                IsHtml = true
            }, cancellationToken);
        }
        catch
        {
            // Email delivery should not block authentication success.
        }
    }
}



