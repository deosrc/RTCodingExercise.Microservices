using Paging.Domain;

namespace Catalog.Domain;
public record ListRequest
{
    public PagingOptions Paging { get; set; } = new();
}
