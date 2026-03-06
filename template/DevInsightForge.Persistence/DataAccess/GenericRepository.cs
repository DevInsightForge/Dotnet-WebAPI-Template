using DevInsightForge.Application.Abstructions.DataAccess;
using DevInsightForge.Application.DtoModels.Common;
using DevInsightForge.Domain.Entities.Base;
using DevInsightForge.Persistence.DataAccess.Extensions;
using DevInsightForge.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DevInsightForge.Persistence.DataAccess;

public class GenericRepository<TEntity>(DatabaseContext dbContext) : IGenericRepository<TEntity> where TEntity : BaseEntity
{
    protected readonly DbSet<TEntity> _dbSet = dbContext.Set<TEntity>() ?? throw new ArgumentNullException(nameof(dbContext), "Database context cannot be null.");

    public async Task AddAsync(TEntity entity, CancellationToken ct = default)
    {
        await _dbSet.AddAsync(entity, ct);
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken ct = default)
    {
        await Task.FromResult(_dbSet.Update(entity));
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default)
    {
        await _dbSet.AddRangeAsync(entities, ct);
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.AsNoTracking().AnyAsync(predicate);
    }

    public async Task<TEntity?> GetByIdAsync(dynamic id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<TEntity?> GetWhereAsync(Expression<Func<TEntity, bool>> where, params Expression<Func<TEntity, object>>[] include)
    {
        return await _dbSet.AsNoTracking().AsQueryable().IncludeMultiple(include).FirstOrDefaultAsync(where);
    }

    public async Task<PaginatedDto<TEntity>> GetAllAsync(int pageNumber, int pageSize, params Expression<Func<TEntity, object>>[] include)
    {
        return await _dbSet.AsNoTracking().IncludeMultiple(include).GetPaginatedResultAsync(pageNumber, pageSize);
    }

    public async Task<PaginatedDto<TEntity>> GetAllWhereAsync(int pageNumber, int pageSize, Expression<Func<TEntity, bool>> where, params Expression<Func<TEntity, object>>[] include)
    {
        return await _dbSet.Where(where).AsNoTracking().IncludeMultiple(include).GetPaginatedResultAsync(pageNumber, pageSize);
    }
}




