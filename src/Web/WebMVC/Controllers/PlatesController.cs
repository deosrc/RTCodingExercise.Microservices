using RTCodingExercise.Microservices.Services;

namespace RTCodingExercise.Microservices.Controllers;
public class PlatesController : Controller
{
    private readonly ICatalogService _catalogService;

    public PlatesController(ICatalogService catalogService)
    {
        _catalogService = catalogService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
    {
        var plates = await _catalogService.GetPlatesAsync(cancellationToken);
        return View(plates);
    }

    [HttpGet]
    public IActionResult Add()
    {
        return View();
    }
}
