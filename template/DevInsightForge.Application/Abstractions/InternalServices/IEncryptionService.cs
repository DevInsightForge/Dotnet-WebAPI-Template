namespace DevInsightForge.Application.Abstractions.InternalServices;

public interface IEncryptionService
{
    string HashPassword(string password);
    bool VerifyPassword(string hashedPassword, string providedPassword);
}


