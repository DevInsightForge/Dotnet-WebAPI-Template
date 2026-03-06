using DevInsightForge.Domain.Entities.Base;

namespace DevInsightForge.Domain.Entities;

public class UserModel : BaseEntity
{
    public string Email { get; private set; } = string.Empty;
    public string NormalizedEmail { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public bool IsEmailVerified { get; private set; } = false;
    public DateTime DateJoined { get; private set; }
    public DateTime LastLogin { get; private set; }


    // Private constructor
    private UserModel()
    {
        DateJoined = DateTime.UtcNow;
        LastLogin = DateJoined;
    }

    // Factory method
    public static UserModel CreateUser(string email)
    {
        UserModel user = new();
        return user.SetEmail(email);
    }

    public UserModel SetEmail(string email)
    {
        ArgumentException.ThrowIfNullOrEmpty(email.Trim(), nameof(email));

        Email = email;
        NormalizedEmail = email.ToUpperInvariant();

        return this;
    }

    public UserModel SetPasswordHash(string password)
    {
        ArgumentException.ThrowIfNullOrEmpty(password.Trim(), nameof(password));

        PasswordHash = password;
        return this;
    }

    public UserModel MarkEmailAsVerified()
    {
        IsEmailVerified = true;
        return this;
    }

    public UserModel UpdateLastLogin()
    {
        LastLogin = DateTime.UtcNow;
        return this;
    }
}
