using DevInsightForge.Application.Abstructions.DataAccess;
using DevInsightForge.Application.Abstructions.DataAccess.Repositories;
using DevInsightForge.Persistence.DataAccess.Repositories;
using DevInsightForge.Persistence.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace DevInsightForge.Persistence.DataAccess;

public class UnitOfWork(DatabaseContext databaseContext, ILogger<UnitOfWork> logger) : IUnitOfWork
{
    private readonly Lazy<IUserRepository> _users = new(() => new UserRepository(databaseContext));
    public IUserRepository Users => _users.Value;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        databaseContext.SaveChangesAsync(cancellationToken);

    public async Task WithTransaction(Func<CancellationToken, Task> operation, CancellationToken cancellationToken = default)
    {
        if (databaseContext.Database.CurrentTransaction is not null)
        {
            await operation(cancellationToken);
            return;
        }

        var strategy = databaseContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async ct =>
        {
            await using IDbContextTransaction transaction = await databaseContext.Database.BeginTransactionAsync(ct);
            try
            {
                await operation(ct);
                await transaction.CommitAsync(ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error executing transactional operation.");
                await transaction.RollbackAsync(ct);
                throw;
            }
        }, cancellationToken);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            databaseContext.Dispose();
        }
    }
}
