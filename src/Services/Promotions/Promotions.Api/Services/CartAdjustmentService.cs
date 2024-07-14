using Promotions.Api.Data;
using Promotions.Api.Services.PromotionTypes;
using Promotions.Domain;

namespace Promotions.Api.Services;

public class CartAdjustmentService(IPromotionsRepository promotionsRepository, ILogger<CartAdjustmentService> logger) : ICartAdjustmentService
{
    private readonly IPromotionsRepository _promotionsRepository = promotionsRepository;
    private readonly ILogger<CartAdjustmentService> _logger = logger;

    public async Task<PromotionApplyResult> TryApplyPromotionAsync(Cart cart, CancellationToken cancellationToken = default)
    {
        var promotion = await _promotionsRepository.GetCurrentPromotionByCodeAsync(cart.PromotionCode, cancellationToken);
        if (promotion is null)
            return new()
            {
                IsSuccess = false,
                Message = "Promotion code is not valid."
            };

        IPromotionTypeService promotionTypeService = promotion.Type switch
        {
            _ => throw new InvalidOperationException("Unrecognised promotion type.")
        };

        return promotionTypeService.TryApplyPromotion(cart, promotion.Options);
    }
}
