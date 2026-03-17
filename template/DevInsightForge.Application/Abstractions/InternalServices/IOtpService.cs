namespace DevInsightForge.Application.Abstractions.InternalServices;

public interface IOtpService
{
    (string OtpCode, DateTime ExpiresAtUtc) GenerateEmailVerificationOtp(string email);
    bool VerifyEmailVerificationOtp(string email, string otp);
}


