using DevInsightForge.Domain.Entities;
using DevInsightForge.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevInsightForge.Persistence.Configurations;

internal class UserConfiguration : BaseEntityConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder.ToTable("Users");

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(u => u.Email);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.IsEmailVerified)
            .IsRequired();

        builder.Property(u => u.DateJoined)
            .IsRequired();

        builder.Property(u => u.LastLogin)
            .IsRequired();

        builder.HasData(new
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Email = "admin@system.local",
            PasswordHash = "$2a$12$ByCcav7akmgD92OJcyegQe38aeIWvJj0wroOQjCKo0MG7nL3Yh7Qa",
            IsEmailVerified = true,
            DateJoined = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            LastLogin = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}
