using DevInsightForge.Domain.Entities.Base;

namespace DevInsightForge.Domain.Entities;

public class Role : BaseAuditableEntity
{
    public string Name { get; private set; } = string.Empty;

    private Role()
    {
    }

    public static Role Create(string name)
    {
        return new Role().SetName(name);
    }

    public Role SetName(string name)
    {
        var normalizedName = name.Trim();
        ArgumentException.ThrowIfNullOrEmpty(normalizedName, nameof(name));
        Name = normalizedName;
        return this;
    }
}
