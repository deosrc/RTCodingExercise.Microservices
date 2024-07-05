using Catalog.API.Data.Repositories;
using System.Net;

namespace Catalog.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PlatesController : ControllerBase
{
    private IPlateRepository _platesRepository;

    public PlatesController(IPlateRepository platesRepository)
    {
        _platesRepository = platesRepository;
    }

    [HttpGet("")]
    [ProducesResponseType(typeof(IEnumerable<Plate>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> List(CancellationToken cancellationToken = default)
    {
        return new OkObjectResult(await _platesRepository.GetPlatesAsync(cancellationToken));
    }
}
