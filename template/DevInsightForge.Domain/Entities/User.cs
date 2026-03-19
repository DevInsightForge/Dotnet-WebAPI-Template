using DevInsightForge.Domain.Entities.Base;

namespace DevInsightForge.Domain.Entities;

public class User : BaseEntity
{
    public Guid RoleId { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public DateTime DateJoined { get; private set; }
    public DateTime LastLogin { get; private set; }
    public virtual Role? Role { get; }

    private User()
    {
        DateJoined = DateTime.UtcNow;
        LastLogin = DateJoined;
    }

    public static User Create(string email, Guid roleId)
    {
        var user = new User();
        return user.SetEmail(email).SetRoleId(roleId);
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

    public User SetRoleId(Guid roleId)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(roleId, Guid.Empty, nameof(roleId));
        RoleId = roleId;
        return this;
    }

    public User UpdateLastLogin()
    {
        LastLogin = DateTime.UtcNow;
        return this;
    }
}

