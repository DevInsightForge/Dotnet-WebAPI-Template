namespace DevInsightForge.Application.Abstructions;

public interface IEncryptionService
{
    string HashPassword(string password);
    bool VerifyPassword(string hashedPassword, string providedPassword);
}
