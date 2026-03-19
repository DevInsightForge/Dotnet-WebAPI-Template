using DevInsightForge.Application.Abstractions.DataAccess.Repositories;

namespace DevInsightForge.Application.Abstractions.DataAccess;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IRoleRepository Roles { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task WithTransaction(Func<CancellationToken, Task> operation, CancellationToken cancellationToken = default);
}





