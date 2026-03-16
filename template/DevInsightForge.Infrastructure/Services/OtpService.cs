using DevInsightForge.Application.Abstructions;
using DevInsightForge.Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace DevInsightForge.Infrastructure.Services;

public class OtpService(IOptions<ApplicationConfiguration> applicationOptions) : IOtpService
{
    private static readonly TimeSpan OtpLifetime = TimeSpan.FromMinutes(10);
    private static readonly TimeSpan OtpTimeStep = TimeSpan.FromMinutes(5);
    private readonly string _otpSecretKey = applicationOptions.Value.EncryptionSecretKey;

    public (string OtpCode, DateTime ExpiresAtUtc) GenerateEmailVerificationOtp(string email)
    {
        var now = DateTime.UtcNow;
        var normalizedEmail = NormalizeEmail(email);
        var bucket = GetTimeBucket(now);
        var otpCode = GenerateOtpCode(normalizedEmail, bucket);
        var expiresAtUtc = now.Add(OtpLifetime);
        return (otpCode, expiresAtUtc);
    }

    public bool VerifyEmailVerificationOtp(string email, string otp)
    {
        var normalizedEmail = NormalizeEmail(email);
        var trimmedOtp = otp.Trim();
        if (trimmedOtp.Length != 6 || !trimmedOtp.All(char.IsDigit))
        {
            return false;
        }

        var nowBucket = GetTimeBucket(DateTime.UtcNow);
        var bucketsToCheck = OtpLifetime.Ticks / OtpTimeStep.Ticks;

        for (var i = 0; i <= bucketsToCheck; i++)
        {
            var candidate = GenerateOtpCode(normalizedEmail, nowBucket - i);
            if (CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(candidate),
                Encoding.UTF8.GetBytes(trimmedOtp)))
            {
                return true;
            }
        }

        return false;
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    private long GetTimeBucket(DateTime utcNow) =>
        utcNow.Ticks / OtpTimeStep.Ticks;

    private string GenerateOtpCode(string normalizedEmail, long timeBucket)
    {
        var payload = $"{normalizedEmail}:{timeBucket}";
        var keyBytes = Encoding.UTF8.GetBytes(_otpSecretKey);
        var payloadBytes = Encoding.UTF8.GetBytes(payload);
        using var hmac = new HMACSHA256(keyBytes);
        var hash = hmac.ComputeHash(payloadBytes);

        var offset = hash[^1] & 0x0F;
        var binaryCode = ((hash[offset] & 0x7F) << 24)
                       | (hash[offset + 1] << 16)
                       | (hash[offset + 2] << 8)
                       | hash[offset + 3];

        var otp = binaryCode % 1_000_000;
        return otp.ToString("D6");
    }
}
