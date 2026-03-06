using DevInsightForge.Domain.Entities.Core;
using DevInsightForge.Persistence.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevInsightForge.Persistence.Persistence.Configurations;

internal class UserModelConfiguration : BaseEntityConfiguration<UserModel>
{
    public override void Configure(EntityTypeBuilder<UserModel> builder)
    {
        base.Configure(builder);

        builder.ToTable("Users");

        builder.Property(u => u.Id)
            .ValueGeneratedOnAdd();

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
    }
}
