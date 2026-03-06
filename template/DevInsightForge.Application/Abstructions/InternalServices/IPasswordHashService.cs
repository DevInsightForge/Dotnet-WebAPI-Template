using DevInsightForge.Domain.Entities.Core;

namespace DevInsightForge.Application.Abstructions;

public interface IPasswordHashService
{
    string HashPassword(UserModel user, string password);
    bool VerifyHashedPassword(UserModel user, string hashedPassword, string providedPassword);
}


