using DevInsightForge.Application.DtoModels.Common;
using DevInsightForge.Domain.Entities.Base;
using System.Linq.Expressions;

namespace DevInsightForge.Application.Abstructions.DataAccess;

public interface IGenericRepository<TEntity> where TEntity : BaseEntity
{
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);

    Task<TEntity?> GetByIdAsync(dynamic id);

    Task<TEntity?> GetWhereAsync(
        Expression<Func<TEntity, bool>> where,
        params Expression<Func<TEntity, object>>[] include);

    Task<PaginatedResponseModel<TEntity>> GetAllAsync(
        int pageNumber,
        int pageSize,
        params Expression<Func<TEntity, object>>[] include);

    Task<PaginatedResponseModel<TEntity>> GetAllWhereAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<TEntity, bool>> where,
        params Expression<Func<TEntity, object>>[] include);
}



