using DevInsightForge.Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevInsightForge.Persistence.Configurations.Base;

public abstract class BaseAuditableEntityConfiguration<TBase> : BaseEntityConfiguration<TBase>
where TBase : BaseAuditableEntity
{
    public override void Configure(EntityTypeBuilder<TBase> builder)
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
