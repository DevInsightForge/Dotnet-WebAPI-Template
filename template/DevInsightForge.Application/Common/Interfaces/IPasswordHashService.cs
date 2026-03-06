using DevInsightForge.Domain.Entities.Core;

namespace DevInsightForge.Application.Common.Interfaces;

public interface IPasswordHashService
{
    string HashPassword(UserModel user, string password);
    bool VerifyHashedPassword(UserModel user, string hashedPassword, string providedPassword);
}
