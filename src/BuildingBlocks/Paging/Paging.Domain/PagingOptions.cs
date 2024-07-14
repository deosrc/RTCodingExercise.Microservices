using System.ComponentModel.DataAnnotations;

namespace Paging.Domain;
public record PagingOptions
{
    public const int DefaultItemsPerPage = 20;

    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(10, 250)]
    public int ItemsPerPage { get; set; } = DefaultItemsPerPage;
}
