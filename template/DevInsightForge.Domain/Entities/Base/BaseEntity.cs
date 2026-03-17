namespace DevInsightForge.Domain.Entities.Base;

public abstract class BaseEntity
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public bool IsDeleted { get; private set; } = false;

    public void MarkAsDeleted()
    {
        IsDeleted = true;
    }

    public void Restore()
    {
        IsDeleted = false;
    }
}

