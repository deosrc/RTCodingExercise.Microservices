using Promotions.Domain;

namespace Promotions.Api.Services.PromotionTypes;

public class MoneyOffPromotion(ILogger<MoneyOffPromotion> logger) : IPromotionTypeService
{
    private ILogger<MoneyOffPromotion> logger = logger;

    public PromotionApplyResult TryApplyPromotion(Cart cart, string promotionId, IReadOnlyDictionary<string, string> promotionOptions)
    {
        throw new NotImplementedException();
    }
}
