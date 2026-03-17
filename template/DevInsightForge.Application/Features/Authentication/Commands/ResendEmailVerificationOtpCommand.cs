using DevInsightForge.Application.Abstractions.DataAccess;
using DevInsightForge.Application.Abstractions.ExternalServices;
using DevInsightForge.Application.Abstractions.InternalServices;
using DevInsightForge.Application.Contracts.Authentication;
using DevInsightForge.Application.Contracts.Common;
using DevInsightForge.Application.Results;

namespace DevInsightForge.Application.Features.Authentication.Commands;

public sealed record ResendEmailVerificationOtpCommand(ResendEmailVerificationOtpRequestDto Request) : IRequest<ResendEmailVerificationOtpCommand, Task<Result>>;

internal sealed class ResendEmailVerificationOtpCommandHandler(
    IOtpService otpService,
    IEmailService emailService,
    IUnitOfWork unitOfWork,
    IValidator<ResendEmailVerificationOtpRequestDto> validator) : IRequestHandler<ResendEmailVerificationOtpCommand, Task<Result>>
{
    public async Task<Result> Handle(ResendEmailVerificationOtpCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request.Request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.ValidationFailure(validationResult);
        }

        var normalizedEmail = request.Request.Email.Trim().ToLowerInvariant();
        var user = await unitOfWork.Users.GetWhereAsync(u => u.Email == normalizedEmail);
        if (user is null)
        {
            return Result.Failure(new Error("user.not_found", "User not found.", ErrorType.NotFound));
        }

        if (user.IsEmailVerified)
        {
            return Result.Failure(new Error("auth.already_verified", "Email is already verified.", ErrorType.Conflict));
        }

        var (otpCode, expiresAtUtc) = otpService.GenerateEmailVerificationOtp(user.Email);
        await SendVerificationEmailAsync(emailService, user.Email, otpCode, expiresAtUtc, cancellationToken);
        return Result.Success();
    }

    private static async Task SendVerificationEmailAsync(
        IEmailService emailService,
        string userEmail,
        string otpCode,
        DateTime expiresAtUtc,
        CancellationToken cancellationToken)
    {
        var ttlMinutes = Math.Max(1, (int)Math.Ceiling((expiresAtUtc - DateTime.UtcNow).TotalMinutes));
        var emailBody = $"""
            <body>
                <main>
                    <section>
                        <p>Dear User,</p>
                        <p>Your DevInsightForge verification OTP is:</p>
                        <h2>{otpCode}</h2>
                        <p>This OTP expires in approximately {ttlMinutes} minute(s).</p>
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
                Subject = "DevInsightForge email verification OTP",
                Body = emailBody,
                IsHtml = true
            }, cancellationToken);
        }
        catch
        {
            // OTP emails are best effort; clients can trigger resend again.
        }
    }
}



