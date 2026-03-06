namespace DevInsightForge.Application.Abstructions.Core;

public interface IRequestContextService
{
    Guid? RequestUserId { get; }
}
