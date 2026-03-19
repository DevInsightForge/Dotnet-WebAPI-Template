using DevInsightForge.Application.Abstractions.DataAccess;
using DevInsightForge.Application.Abstractions.DataAccess.Repositories;
using DevInsightForge.Persistence.DataAccess.Repositories;
using DevInsightForge.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace DevInsightForge.Persistence.DataAccess;

public class UnitOfWork(DatabaseContext databaseContext, ILogger<UnitOfWork> logger) : IUnitOfWork
{
    private readonly Lazy<IUserRepository> _users = new(() => new UserRepository(databaseContext));
    private readonly Lazy<IRoleRepository> _roles = new(() => new RoleRepository(databaseContext));
    public IUserRepository Users => _users.Value;
    public IRoleRepository Roles => _roles.Value;

    public Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        databaseContext.SaveChangesAsync(ct);

    public async Task WithTransaction(Func<CancellationToken, Task> operation, CancellationToken ct = default)
    {
        if (databaseContext.Database.CurrentTransaction is not null)
        {
            await operation(ct);
            return;
        }

        var strategy = databaseContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async internalCt =>
        {
            await using IDbContextTransaction transaction = await databaseContext.Database.BeginTransactionAsync(internalCt);
            try
            {
                await operation(internalCt);
                await transaction.CommitAsync(internalCt);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error executing transactional operation.");
                await transaction.RollbackAsync(internalCt);
                throw;
            }
        }, ct);
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


