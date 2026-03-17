using DevInsightForge.Domain.Entities.Base;

namespace DevInsightForge.Application.Contracts.Common;

public class PaginatedDto<TEntity> where TEntity : BaseEntity
{
    public int TotalRecords { get; set; }
    public int CurrentPageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
    public bool HasNextPage => CurrentPageNumber < TotalPages;
    public bool HasPreviousPage => CurrentPageNumber > 1;
    public IEnumerable<TEntity> Data { get; set; } = [];
}





