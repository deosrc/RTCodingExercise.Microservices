namespace Paging.Domain;

public record PageInfo : PagingOptions
{
    public PageInfo()
    {
        // Nothing to do.
    }

    public PageInfo(PagingOptions options, bool hasMorePages)
        : this()
    {
        Page = options.Page;
        ItemsPerPage = options.ItemsPerPage;
        HasNext = hasMorePages;
    }

    public bool HasPrevious => Page > 1;
    public bool HasNext { get; set; }
}