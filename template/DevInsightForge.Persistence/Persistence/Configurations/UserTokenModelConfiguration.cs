using DevInsightForge.Domain.Entities.Core;
using DevInsightForge.Persistence.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevInsightForge.Persistence.Persistence.Configurations;

internal class UserTokenModelConfiguration : BaseEntityConfiguration<UserTokenModel>
{
    public override void Configure(EntityTypeBuilder<UserTokenModel> builder)
    {
        base.Configure(builder);

        builder.ToTable("UserTokens");

        builder.HasOne<UserModel>()
            .WithMany()
            .HasForeignKey(e => e.UserId);

        builder.Property(e => e.UserId)
            .IsRequired();

        builder.Property(e => e.RefreshToken)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.ExpiresAt)
            .IsRequired();

        builder.Property(e => e.IsRevoked)
            .IsRequired();
    }
}
