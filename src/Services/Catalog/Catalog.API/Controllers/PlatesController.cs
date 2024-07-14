using Catalog.API.Data.Repositories;
using Catalog.API.Services.SalesPriceMarkup;
using Paging.Domain;
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
    [ProducesResponseType(typeof(PagedResult<Plate>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> List([FromQuery] ListRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _platesRepository.GetPlatesAsync(request.Paging, cancellationToken);
        foreach (var p in result.Results)
        {
            _salesPriceMarkupService.AddSalesPriceMarkup(p);
        }
        return new OkObjectResult(result);
    }

    [HttpPost("")]
    [ProducesResponseType(typeof(Plate), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> Add(NewPlate plate, CancellationToken cancellationToken = default)
    {
        var result = await _platesRepository.AddPlateAsync(plate, cancellationToken);
        if (!result.IsSuccess)
            return Problem(
                title: "Operation failed",
                detail: result.Message,
                statusCode: (int)HttpStatusCode.InternalServerError);

        return Ok(result.Result);
    }
}
