namespace Catalog.Domain;
public record PagedResult<TResult>
    where TResult : class
{
    public PagedResult()
    {
        // Nothing to do.
    }

    public PagedResult(IEnumerable<TResult> results, PageInfo pageInfo)
        : this()
    {
        Results = results;
        Paging = pageInfo;
    }

    public IEnumerable<TResult> Results { get; set; } = Array.Empty<TResult>();

    public PageInfo Paging { get; set; } = new();
}
