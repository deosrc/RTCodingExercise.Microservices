using Microsoft.AspNetCore.Mvc;

namespace Promotions.Api.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class HomeController : Controller
{
    [HttpGet("/")]
    public IActionResult Index()
    {
        return new RedirectResult("~/swagger");
    }
}
