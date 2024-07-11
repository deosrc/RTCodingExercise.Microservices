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
    [ProducesResponseType(typeof(PagedResult<Plate>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> List(ListRequest request, CancellationToken cancellationToken = default)
    {
        return new OkObjectResult(await _platesRepository.GetPlatesAsync(request.Paging, cancellationToken));
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
