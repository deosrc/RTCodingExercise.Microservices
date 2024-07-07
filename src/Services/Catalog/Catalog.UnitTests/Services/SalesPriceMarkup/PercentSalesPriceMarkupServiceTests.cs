using Catalog.API.Services.SalesPriceMarkup;
using Catalog.Domain;
using Xunit;

namespace Catalog.UnitTests.Services.SalesPriceMarkup;
public class PercentSalesPriceMarkupServiceTests
{
    [Theory]
    [InlineData(5, 100, 105)]
    [InlineData(5, 200, 210)]
    [InlineData(5, 300, 315)]
    [InlineData(10, 100, 110)]
    [InlineData(20, 100, 120)]
    public void AddSalesPriceMarkup(int percentMarkup, decimal salesPrice, decimal expected)
    {
        var plate = new Plate()
        {
            SalePrice = salesPrice
        };

        var sut = new PercentSalesPriceMarkupService(new PercentSalesPriceMarkupOptions()
        {
            PercentMarkup = percentMarkup,
        });

        sut.AddSalesPriceMarkup(plate);

        Assert.Equal(expected, plate.SalePrice);
        Assert.Equal($"{percentMarkup}%", plate.SalesPriceMarkup);
    }
}
