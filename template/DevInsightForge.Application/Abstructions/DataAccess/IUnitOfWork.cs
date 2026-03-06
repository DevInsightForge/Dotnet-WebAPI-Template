using DevInsightForge.Application.Abstructions.DataAccess.Repositories;

namespace DevInsightForge.Application.Abstructions.DataAccess;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task WithTransaction(Func<CancellationToken, Task> operation, CancellationToken ct = default);
}



