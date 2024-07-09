namespace Catalog.Domain;
public class PagedResult<TResult>
    where TResult : class
{
    public PagedResult(IEnumerable<TResult> results, PageInfo pageInfo)
    {
        Results = results;
        Paging = pageInfo;
    }

    public IEnumerable<TResult> Results { get; }

    public PageInfo Paging { get; }
}
