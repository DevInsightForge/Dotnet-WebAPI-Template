using DevInsightForge.Domain.Entities;
using DevInsightForge.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevInsightForge.Persistence.Configurations;

internal class UserModelConfiguration : BaseEntityConfiguration<UserModel>
{
    public override void Configure(EntityTypeBuilder<UserModel> builder)
    {
        base.Configure(builder);

        builder.ToTable("Users");

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.NormalizedEmail)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(u => u.NormalizedEmail)
            .IsUnique();

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
            Id = Guid.Parse("019cc42b-1d4a-7e16-886c-5267c7e96651"),
            Email = "admin@default.local",
            NormalizedEmail = "ADMIN@DEFAULT.LOCAL",
            PasswordHash = "$argon2id$v=19$m=65536,t=3,p=1$Xka0Ez/kddlgKLbErxj7Ng$mBT9xHzRHIhVfsL3kV79DzB2TIL/mMhXp5SbVHBMzTc",
            IsEmailVerified = true,
            DateJoined = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            LastLogin = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}
