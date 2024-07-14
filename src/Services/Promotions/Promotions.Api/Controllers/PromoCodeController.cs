using Microsoft.AspNetCore.Mvc;
using Promotions.Api.Services;
using Promotions.Domain;
using System.Net;

namespace Promotions.Api.Controllers;
[ApiController]
[Route("[controller]")]
public class PromoCodeController(ICartAdjustmentService cartAdjustmentService, ILogger<PromoCodeController> logger) : ControllerBase
{
    private readonly ICartAdjustmentService _cartAdjustmentService = cartAdjustmentService;
    private readonly ILogger<PromoCodeController> _logger = logger;

    /// <summary>
    /// Apply a promotion to a cart.
    /// </summary>
    /// <param name="cart"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("apply")]
    public async Task<IActionResult> Apply(Cart cart, CancellationToken cancellationToken = default)
    {
        return Ok(await _cartAdjustmentService.TryApplyPromotionAsync(cart, cancellationToken));
    }
}
