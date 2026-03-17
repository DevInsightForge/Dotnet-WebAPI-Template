namespace DevInsightForge.Application.Abstractions;

public interface IEncryptionService
{
    string HashPassword(string password);
    bool VerifyPassword(string hashedPassword, string providedPassword);
}


