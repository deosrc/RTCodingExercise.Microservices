using Promotions.Domain;
using RTCodingExercise.Microservices.Services.Promotions;

namespace RTCodingExercise.Microservices.Controllers;
public class CartController : Controller
{
    private readonly IPromotionsApiService _promotionService;

    public CartController(IPromotionsApiService promotionService)
    {
        _promotionService = promotionService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(decimal platePrice, string promotionCode, CancellationToken cancellationToken = default)
    {
        var exampleCart = new Cart()
        {
            CartItems = new List<CartItem>()
            {
                new CartItem()
                {
                    PlateId = Guid.NewGuid(),
                    Price = platePrice
                }
            },
            PromotionCode = promotionCode,
        };
        var result = await _promotionService.ApplyPromotionAsync(exampleCart, cancellationToken);
        return View(result);
    }
}
