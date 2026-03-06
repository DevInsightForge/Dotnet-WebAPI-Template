namespace DevInsightForge.Application.Abstructions;

public interface IEncryptionService
{
    string HashPassword(string password);
    bool VerifyPassword(string hashedPassword, string providedPassword);
    (string OtpCode, DateTime ExpiresAtUtc) GenerateEmailVerificationOtp(string email);
    bool VerifyEmailVerificationOtp(string email, string otp);
}
