using System.Linq.Expressions;
using DevInsightForge.Application.Contracts.Common;
using DevInsightForge.Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace DevInsightForge.Persistence.DataAccess.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<TEntity> IncludeNavigations<TEntity>(
        this IQueryable<TEntity> query,
        params Expression<Func<TEntity, object>>[] includes) where TEntity : BaseEntity
    {
        if (includes != null && includes.Length != 0)
        {
            query = includes.Aggregate(query, (current, expression) => current.Include(expression));
        }
        return query;
    }

    public static async Task<PaginatedDto<TEntity>> ToPaginatedResultAsync<TEntity>(
        this IQueryable<TEntity> query, int pageNumber, int pageSize) where TEntity : BaseEntity
    {
        var totalRecords = await query.CountAsync();
        var data = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedDto<TEntity>
        {
            TotalRecords = totalRecords,
            CurrentPageNumber = pageNumber,
            PageSize = pageSize,
            Data = data
        };
    }
}




