using Catalog.API.Data.Repositories;

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
    public Task<IActionResult> List(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
