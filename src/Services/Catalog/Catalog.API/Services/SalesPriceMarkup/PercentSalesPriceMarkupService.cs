
namespace Catalog.API.Services.SalesPriceMarkup;

public class PercentSalesPriceMarkupService : ISalesPriceMarkupService
{
    private readonly PercentSalesPriceMarkupOptions _options;

    public PercentSalesPriceMarkupService(IOptions<PercentSalesPriceMarkupOptions> options)
        : this(options.Value)
    {
        // Nothing to do.
    }

    public PercentSalesPriceMarkupService(PercentSalesPriceMarkupOptions options)
    {
        _options = options;
    }

    public void AddSalesPriceMarkup(Plate plate)
    {
        throw new NotImplementedException();
    }
}
