using DevInsightForge.Domain.Entities;

namespace DevInsightForge.Domain.Entities.Base;

public abstract class BaseAuditableEntity : BaseEntity
{
    public Guid? CreatedByUserId { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public Guid? ModifiedByUserId { get; private set; }
    public DateTime ModifiedOn { get; private set; }

    #region Foreign Key Relations
    public virtual UserModel? CreatedByUser { get; }
    public virtual UserModel? ModifiedByUser { get; }
    #endregion

    public void SetCreationAudit(Guid? createdByUserId)
    {
        if (createdByUserId is null) return;

        CreatedByUserId = createdByUserId;
        CreatedOn = DateTime.UtcNow;
    }

    public void SetModificationAudit(Guid? modifiedByUserId)
    {
        if (modifiedByUserId is null) return;

        ModifiedByUserId = modifiedByUserId;
        ModifiedOn = DateTime.UtcNow;
    }

    public bool HasBeenModified() => CreatedOn != ModifiedOn;
}
