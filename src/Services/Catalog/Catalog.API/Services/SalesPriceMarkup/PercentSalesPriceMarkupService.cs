namespace Catalog.API.Services.SalesPriceMarkup;

public class PercentSalesPriceMarkupService : ISalesPriceMarkupService
{
    private readonly PercentSalesPriceMarkupOptions _options;
    private readonly decimal _salesPriceMultiplier;

    public PercentSalesPriceMarkupService(IOptions<PercentSalesPriceMarkupOptions> options)
        : this(options.Value)
    {
        // Nothing to do.
    }

    public PercentSalesPriceMarkupService(PercentSalesPriceMarkupOptions options)
    {
        _options = options;

        // Convert to multiplier. For example, 1.1 is 110% of original sale price.
        _salesPriceMultiplier = (_options.PercentMarkup / 100.0M) + 1;
    }

    public void AddSalesPriceMarkup(Plate plate)
    {
        plate.SalePrice *= _salesPriceMultiplier;
        plate.SalesPriceMarkup = $"{_options.PercentMarkup}%";
    }
}
