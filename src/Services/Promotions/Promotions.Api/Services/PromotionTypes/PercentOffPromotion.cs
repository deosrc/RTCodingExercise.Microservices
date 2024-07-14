using Promotions.Domain;

namespace Promotions.Api.Services.PromotionTypes;

public class PercentOffPromotion(ILogger<PercentOffPromotion> logger) : IPercentOffPromotion, IPromotionTypeService
{
    private const string OptionPercentOff = "PercentOff";

    private ILogger<PercentOffPromotion> logger = logger;

    public PromotionApplyResult TryApplyPromotion(Cart cart, string promotionId, IReadOnlyDictionary<string, string> promotionOptions)
    {
        var percentOffRaw = promotionOptions.GetValueOrDefault(OptionPercentOff)
            ?? throw new InvalidOperationException("Promotion is not configured correctly: Missing percent off");

        if (!int.TryParse(percentOffRaw, out var percentOff))
            throw new InvalidOperationException("Promotion is not configured correctly: Invalid format for percent off.");

        if (percentOff > 50 || percentOff < 1)
            throw new InvalidOperationException("Promotion is not configured correctly: Percent off must be between 1 and 50.");

        var cartTotal = cart.CartItems.Sum(x => x.Price);
        return new PromotionApplyResult
        {
            IsSuccess = true,
            Message = "Promotion applied",
            PromotionCartItems = [
                new PromotionCartItem()
                {
                    PromotionId = promotionId,
                    Description = string.Format("{0}% off", percentOff),
                    Discount = decimal.Round(cartTotal * (percentOff / 100.0M), 2)
                }
            ]
        };
    }
}
