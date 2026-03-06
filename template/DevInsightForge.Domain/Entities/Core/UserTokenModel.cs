using DevInsightForge.Domain.Entities.Common;
using System.Security.Cryptography;

namespace DevInsightForge.Domain.Entities.Core;

public class UserTokenModel : BaseEntity
{
    public Guid UserId { get; private set; }
    public string RefreshToken { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; } = false;

    private UserTokenModel() { }

    public static UserTokenModel Create(Guid userId, DateTime expiresAt)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));

        if (expiresAt <= DateTime.UtcNow)
            throw new ArgumentException("Expiration date must be in the future.", nameof(expiresAt));

        var userToken = GenerateToken(userId);

        return new UserTokenModel
        {
            UserId = userId,
            RefreshToken = userToken,
            ExpiresAt = expiresAt
        };
    }

    public UserTokenModel RevokeToken()
    {
        IsRevoked = true;
        return this;
    }

    private static string GenerateToken(Guid userId)
    {
        byte[] randomNumber = new byte[32];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
        }

        byte[] userIdBytes = userId.ToByteArray();
        byte[] combinedBytes = new byte[randomNumber.Length + userIdBytes.Length];
        Array.Copy(randomNumber, combinedBytes, randomNumber.Length);
        Array.Copy(userIdBytes, 0, combinedBytes, randomNumber.Length, userIdBytes.Length);

        byte[] hashedBytes = SHA256.HashData(combinedBytes);
        string refreshToken = Convert.ToBase64String(hashedBytes);

        return refreshToken;
    }
}
