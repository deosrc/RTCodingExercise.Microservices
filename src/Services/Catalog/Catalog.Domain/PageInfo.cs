namespace Catalog.Domain;

public record PageInfo : PagingOptions
{
    public PageInfo(PagingOptions options, bool hasMorePages)
    {
        Page = options.Page;
        ItemsPerPage = options.ItemsPerPage;
        HasNext = hasMorePages;
    }

    public bool HasPrevious => Page > 1;
    public bool HasNext { get; }
}