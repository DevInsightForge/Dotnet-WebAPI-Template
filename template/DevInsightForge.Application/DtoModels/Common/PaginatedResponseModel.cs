using DevInsightForge.Domain.Entities.Common;

namespace DevInsightForge.Application.DtoModels.Common;

public class PaginatedResponseModel<TEntity> where TEntity : BaseEntity
{
    public int TotalRecords { get; set; }
    public int CurrentPageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
    public bool HasNextPage => CurrentPageNumber < TotalPages;
    public bool HasPreviousPage => CurrentPageNumber > 1;
    public IEnumerable<TEntity> Data { get; set; } = Enumerable.Empty<TEntity>();
}


