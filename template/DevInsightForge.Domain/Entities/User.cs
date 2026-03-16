using DevInsightForge.Domain.Entities.Base;

namespace DevInsightForge.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public bool IsEmailVerified { get; private set; }
    public DateTime DateJoined { get; private set; }
    public DateTime LastLogin { get; private set; }

    private User()
    {
        DateJoined = DateTime.UtcNow;
        LastLogin = DateJoined;
    }

    public static User Create(string email)
    {
        var user = new User();
        return user.SetEmail(email);
    }

    public User SetEmail(string email)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        ArgumentException.ThrowIfNullOrEmpty(normalizedEmail, nameof(email));

        Email = normalizedEmail;

        return this;
    }

    public User SetPasswordHash(string passwordHash)
    {
        ArgumentException.ThrowIfNullOrEmpty(passwordHash.Trim(), nameof(passwordHash));

        PasswordHash = passwordHash;
        return this;
    }

    public User MarkEmailAsVerified()
    {
        IsEmailVerified = true;
        return this;
    }

    public User UpdateLastLogin()
    {
        LastLogin = DateTime.UtcNow;
        return this;
    }
}
