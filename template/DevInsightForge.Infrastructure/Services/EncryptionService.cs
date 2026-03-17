using DevInsightForge.Application.Abstractions.InternalServices;

namespace DevInsightForge.Infrastructure.Services;

public class EncryptionService : IEncryptionService
{
    private const int BcryptWorkFactor = 12;

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: BcryptWorkFactor);
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
    }
}

