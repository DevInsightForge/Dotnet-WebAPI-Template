using DevInsightForge.Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevInsightForge.Persistence.Configurations.Base;

public abstract class BaseAuditableEntityConfiguration<TBase> : IEntityTypeConfiguration<TBase>
where TBase : BaseAuditableEntity
{
    public virtual void Configure(EntityTypeBuilder<TBase> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .ValueGeneratedNever();

        builder.Property(t => t.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasQueryFilter(t => !t.IsDeleted);

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
