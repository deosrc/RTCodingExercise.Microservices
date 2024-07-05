namespace RTCodingExercise.Microservices.Controllers;
public class PlatesController : Controller
{
    public IActionResult Index()
    {
        var plates = new List<Plate>()
        {
            new()
            {
                Registration = "Test Reg",
                PurchasePrice = 123.44M,
                SalePrice = 123.33M,
                Letters = "TEST",
                Numbers = 123
            }
        };
        return View(plates);
    }
}
