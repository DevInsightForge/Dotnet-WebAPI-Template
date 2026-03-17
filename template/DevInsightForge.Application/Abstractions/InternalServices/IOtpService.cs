namespace DevInsightForge.Application.Abstractions;

public interface IOtpService
{
    (string OtpCode, DateTime ExpiresAtUtc) GenerateEmailVerificationOtp(string email);
    bool VerifyEmailVerificationOtp(string email, string otp);
}


