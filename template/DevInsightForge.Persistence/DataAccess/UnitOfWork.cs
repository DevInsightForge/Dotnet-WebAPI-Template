using DevInsightForge.Application.Abstractions.DataAccess;
using DevInsightForge.Persistence.DataContext;

namespace DevInsightForge.Persistence.DataAccess;

public class UnitOfWork(DatabaseContext databaseContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct = default) => databaseContext.SaveChangesAsync(ct);

    public async Task WithTransaction(Func<CancellationToken, Task> operation, CancellationToken ct = default)
    {
        await operation(ct);
        await databaseContext.SaveChangesAsync(ct);
    }

    public void Dispose()
    {
        databaseContext.Dispose();
        GC.SuppressFinalize(this);
    }
}
