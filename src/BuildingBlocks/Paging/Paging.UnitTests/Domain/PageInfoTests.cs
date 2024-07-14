using AutoFixture;
using Paging.Domain;

namespace Paging.UnitTests.Domain;
public class PageInfoTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void Constructor_AppliesPropertiesFromOptions()
    {
        var options = _fixture.Create<PagingOptions>();
        var result = new PageInfo(options, false);

        var properties = typeof(PagingOptions).GetProperties();
        foreach (var property in properties)
        {
            Assert.Equal(property.GetValue(options), property.GetValue(result));
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Constructor_SetsHasNext(bool hasNext)
    {
        var result = new PageInfo(new(), hasNext);
        Assert.Equal(hasNext, result.HasNext);
    }

    [Fact]
    public void HasPrevious_WhenFirstPage_IsFalse()
    {
        var result = new PageInfo(new() { Page = 1 }, true);
        Assert.False(result.HasPrevious);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(10)]
    [InlineData(int.MaxValue)]
    public void HasPrevious_WhenLaterPage_IsTrue(int pageNumber)
    {
        var result = new PageInfo(new() { Page = pageNumber }, true);
        Assert.True(result.HasPrevious);
    }
}
