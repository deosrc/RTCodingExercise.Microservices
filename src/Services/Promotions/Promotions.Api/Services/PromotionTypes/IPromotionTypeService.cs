using Promotions.Domain;

namespace Promotions.Api.Services.PromotionTypes;

public interface IPromotionTypeService
{
    PromotionApplyResult TryApplyPromotion(Cart cart, IReadOnlyDictionary<string, string> promotionOptions);
}
