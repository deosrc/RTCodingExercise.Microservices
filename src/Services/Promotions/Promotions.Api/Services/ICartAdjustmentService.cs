using Promotions.Domain;

namespace Promotions.Api.Services;

/// <summary>
/// Produces instructions for modifying a cart for promotions.
/// </summary>
public interface ICartAdjustmentService
{
    /// <summary>
    /// Attempts to apply a promotion to a cart.
    /// </summary>
    /// <param name="cart"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>One or more instructions for adjusting the cart and displaying to the user, or an empty array if the code is not valid.</returns>
    Task<PromotionApplyResult> TryApplyPromotionAsync(Cart cart, CancellationToken cancellationToken = default);
}
