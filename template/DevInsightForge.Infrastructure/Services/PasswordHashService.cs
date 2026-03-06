using DevInsightForge.Application.Abstructions;
using DevInsightForge.Domain.Entities.Core;
using Microsoft.AspNetCore.Identity;

namespace DevInsightForge.Infrastructure.Services;

public class PasswordHashService(IPasswordHasher<UserModel> passwordHasher) : IPasswordHashService
{
    public string HashPassword(UserModel user, string password)
    {
        return passwordHasher.HashPassword(user, password);
    }

    public bool VerifyHashedPassword(UserModel user, string hashedPassword, string providedPassword)
    {
        return passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword) == PasswordVerificationResult.Success;
    }
}


