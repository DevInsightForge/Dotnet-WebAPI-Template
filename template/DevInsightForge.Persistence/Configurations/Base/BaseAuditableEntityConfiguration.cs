using DevInsightForge.Domain.Entities.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevInsightForge.Persistence.Configurations.Base;

public abstract class BaseAuditableEntityConfiguration<TEntity> : BaseEntityConfiguration<TEntity>
    where TEntity : BaseAuditableEntity
{
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder);

        builder.HasOne(t => t.CreatedByUser)
            .WithMany()
            .HasForeignKey(t => t.CreatedByUserId)
            .IsRequired();

        builder.HasOne(t => t.ModifiedByUser)
            .WithMany()
            .HasForeignKey(t => t.ModifiedByUserId)
            .IsRequired();
    }
}

