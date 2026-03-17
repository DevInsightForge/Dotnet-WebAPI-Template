using DevInsightForge.Application.Abstractions;
using DevInsightForge.Application.Abstractions.DataAccess;
using DevInsightForge.Application.Contracts.Authentication;
using DevInsightForge.Application.Contracts.Common;
using DevInsightForge.Application.Results;
using DevInsightForge.Domain.Entities;

namespace DevInsightForge.Application.Features.Authentication.Commands;

public sealed record RegisterCommand(RegisterRequestDto Request) : IRequest<RegisterCommand, Task<Result>>;

internal sealed class RegisterCommandHandler(
    IEncryptionService encryptionService,
    IOtpService otpService,
    IEmailService emailService,
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

        var (otpCode, expiresAtUtc) = otpService.GenerateEmailVerificationOtp(user.Email);
        await SendVerificationEmailAsync(emailService, user.Email, otpCode, expiresAtUtc, cancellationToken);

        return Result.Created();
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
                        <p>Welcome to <strong>DevInsightForge</strong>. Use the verification code below to verify your account:</p>
                        <h2>{otpCode}</h2>
                        <p>This OTP expires in approximately {ttlMinutes} minute(s).</p>
                    </section>
                    <section>
                        <p>If you did not request this, please ignore this email.</p>
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
                Subject = "Verify your DevInsightForge account",
                Body = emailBody,
                IsHtml = true
            }, cancellationToken);
        }
        catch
        {
            // OTP emails are best effort; user can call resend endpoint.
        }
    }
}



