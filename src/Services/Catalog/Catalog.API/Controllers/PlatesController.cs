using Catalog.API.Data.Repositories;
using Catalog.API.Services.SalesPriceMarkup;
using System.Net;

namespace Catalog.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PlatesController : ControllerBase
{
    private readonly IPlateRepository _platesRepository;
    private readonly ISalesPriceMarkupService _salesPriceMarkupService;

    public PlatesController(IPlateRepository platesRepository, ISalesPriceMarkupService salesPriceMarkupService)
    {
        _platesRepository = platesRepository;
        _salesPriceMarkupService = salesPriceMarkupService;
    }

    [HttpGet("")]
    [ProducesResponseType(typeof(IEnumerable<Plate>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> List(CancellationToken cancellationToken = default)
    {
        var plates = await _platesRepository.GetPlatesAsync(cancellationToken);
        foreach (var p in plates)
        {
            _salesPriceMarkupService.AddSalesPriceMarkup(p);
        }
        return new OkObjectResult(plates);
    }
}
