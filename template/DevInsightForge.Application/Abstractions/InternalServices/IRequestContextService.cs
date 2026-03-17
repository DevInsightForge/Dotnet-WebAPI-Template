namespace DevInsightForge.Application.Abstractions.InternalServices;

public interface IRequestContextService
{
    Guid? RequestUserId { get; }
}


