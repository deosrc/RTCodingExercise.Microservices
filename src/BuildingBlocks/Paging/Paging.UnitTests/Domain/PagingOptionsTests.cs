using Paging.Domain;

namespace Paging.UnitTests.Domain;
public class PagingOptionsTests
{
    [Fact]
    public void Constructor_SetsItemsPerPageDefault()
    {
        var pagingOptions = new PagingOptions();
        Assert.Equal(PagingOptions.DefaultItemsPerPage, pagingOptions.ItemsPerPage);
    }
}
