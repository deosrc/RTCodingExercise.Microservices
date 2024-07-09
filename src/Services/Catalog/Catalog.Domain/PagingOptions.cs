using System.ComponentModel.DataAnnotations;

namespace Catalog.Domain;
public record PagingOptions
{
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(10, 250)]
    public int ItemsPerPage { get; set; } = 20;
}
