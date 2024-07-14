using Promotions.Api.Data;
using Promotions.Api.Data.Models;
using Promotions.Api.Services.PromotionTypes;
using Promotions.Domain;

namespace Promotions.Api.Services;

public class CartAdjustmentService(IPromotionsRepository promotionsRepository, IMoneyOffPromotion moneyOffPromotion, ILogger<CartAdjustmentService> logger) : ICartAdjustmentService
{
    private readonly IPromotionsRepository _promotionsRepository = promotionsRepository;
    private readonly ILogger<CartAdjustmentService> _logger = logger;

    // Promotion types injected via DI. Potential to make this more flexible in future if it becomes more complex.
    private readonly IMoneyOffPromotion _moneyOffPromotion = moneyOffPromotion;

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
            PromotionType.MoneyOff => _moneyOffPromotion,
            _ => throw new InvalidOperationException("Unrecognised promotion type.")
        };

        return promotionTypeService.TryApplyPromotion(cart, promotion.Id.ToString(), promotion.Options);
    }
}
