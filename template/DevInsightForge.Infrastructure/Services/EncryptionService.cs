using DevInsightForge.Application.Abstructions;
using Isopoh.Cryptography.Argon2;

namespace DevInsightForge.Infrastructure.Services;

public class EncryptionService : IEncryptionService
{
    public string HashPassword(string password)
    {
        return Argon2.Hash(password);
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        return Argon2.Verify(hashedPassword, providedPassword);
    }
}
