using DevInsightForge.Application.Abstructions;
using DevInsightForge.Application.Abstructions.DataAccess;
using DevInsightForge.Application.DtoModels.Authentication;
using DevInsightForge.Application.DtoModels.Common;
using DevInsightForge.Application.Results;

namespace DevInsightForge.Application.Features.Authentication.Commands;

public sealed record ResendEmailVerificationOtpCommand(ResendEmailVerificationOtpRequestDto Dto) : IRequest<ResendEmailVerificationOtpCommand, Task<Result>>;

internal sealed class ResendEmailVerificationOtpCommandHandler(
    IEncryptionService encryptionService,
    IEmailService emailService,
    IUnitOfWork unitOfWork,
    IValidator<ResendEmailVerificationOtpRequestDto> validator) : IRequestHandler<ResendEmailVerificationOtpCommand, Task<Result>>
{
    public async Task<Result> Handle(ResendEmailVerificationOtpCommand request, CancellationToken ct)
    {
        var validationResult = await validator.ValidateAsync(request.Dto, ct);
        if (!validationResult.IsValid)
        {
            return Result.ValidationFailure(validationResult);
        }

        var normalizedEmail = request.Dto.Email.Trim().ToUpperInvariant();
        var user = await unitOfWork.Users.GetWhereAsync(u => u.NormalizedEmail == normalizedEmail);
        if (user is null)
        {
            return Result.Failure(new Error("user.not_found", "User not found.", ErrorType.NotFound));
        }

        if (user.IsEmailVerified)
        {
            return Result.Failure(new Error("auth.already_verified", "Email is already verified.", ErrorType.Conflict));
        }

        var (otpCode, expiresAtUtc) = encryptionService.GenerateEmailVerificationOtp(user.Email);
        await SendVerificationEmailAsync(emailService, user.Email, otpCode, expiresAtUtc, ct);
        return Result.Success();
    }

    private static async Task SendVerificationEmailAsync(
        IEmailService emailService,
        string userEmail,
        string otpCode,
        DateTime expiresAtUtc,
        CancellationToken ct)
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
            }, ct);
        }
        catch
        {
            // OTP emails are best effort; clients can trigger resend again.
        }
    }
}
