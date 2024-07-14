using Microsoft.AspNetCore.Mvc;

namespace Promotions.Api.Controllers;
[ApiController]
[Route("[controller]")]
public class PromoCodeController : ControllerBase
{
    private readonly ILogger<PromoCodeController> _logger;

    public PromoCodeController(ILogger<PromoCodeController> logger)
    {
        _logger = logger;
    }
}
