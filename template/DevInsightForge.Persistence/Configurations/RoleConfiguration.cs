using DevInsightForge.Domain.Entities;
using DevInsightForge.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevInsightForge.Persistence.Configurations;

internal class RoleConfiguration : BaseAuditableEntityConfiguration<Role>
{
    public override void Configure(EntityTypeBuilder<Role> builder)
    {
        base.Configure(builder);

        builder.ToTable("Roles");

        builder.Property(r => r.CreatedByUserId)
            .IsRequired(false);

        builder.Property(r => r.ModifiedByUserId)
            .IsRequired(false);

        builder.HasOne(r => r.CreatedByUser)
            .WithMany()
            .HasForeignKey(r => r.CreatedByUserId)
            .IsRequired(false);

        builder.HasOne(r => r.ModifiedByUser)
            .WithMany()
            .HasForeignKey(r => r.ModifiedByUserId)
            .IsRequired(false);

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(r => r.Name);

        builder.HasData(new
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Name = "Super Admin",
            CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            ModifiedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}
