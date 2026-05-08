namespace AcmeTaskApi.Application.Common;
 
/// <summary>
/// Query parameters for paginated list endpoints.
/// </summary>
public record PaginationParams(int Page = 1, int PageSize = 20)
{
    public int Page { get; init; } = Math.Max(1, Page);
    public int PageSize { get; init; } = Math.Clamp(PageSize, 1, 100);
    public int Skip => (Page - 1) * PageSize;
}
 
/// <summary>
/// Generic paginated response envelope.
/// </summary>
public record PagedResult<T>(
    IEnumerable<T> Items,
    int TotalCount,
    int Page,
    int PageSize
)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}
 