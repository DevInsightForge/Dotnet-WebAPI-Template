namespace DevInsightForge.Application.Abstractions.DataAccess;

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task WithTransaction(Func<CancellationToken, Task> operation, CancellationToken cancellationToken = default);
}

