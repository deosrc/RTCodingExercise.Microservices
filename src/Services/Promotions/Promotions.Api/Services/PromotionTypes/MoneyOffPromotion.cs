using Promotions.Domain;
using System.Globalization;

namespace Promotions.Api.Services.PromotionTypes;

public class MoneyOffPromotion(ILogger<MoneyOffPromotion> logger) : IPromotionTypeService
{
    private const string OptionDiscountAmount = "DiscountAmount";

    private static readonly CultureInfo Culture = CultureInfo.CreateSpecificCulture("en-GB");

    private ILogger<MoneyOffPromotion> logger = logger;

    public PromotionApplyResult TryApplyPromotion(Cart cart, string promotionId, IReadOnlyDictionary<string, string> promotionOptions)
    {
        var discountAmountRaw = promotionOptions.GetValueOrDefault(OptionDiscountAmount)
            ?? throw new InvalidOperationException("Promotion is not configured correctly: Missing discount amount");

        if (!decimal.TryParse(discountAmountRaw, out var discountAmount))
            throw new InvalidOperationException("Promotion is not configured correctly: Invalid format for discount amount.");

        var cartTotal = cart.Plates.Sum(x => x.SalePrice);
        if (cartTotal < discountAmount)
            return new PromotionApplyResult
            {
                IsSuccess = false,
                Message = string.Format(Culture, "Promotion requires a cart total of at least {0:C}.", discountAmount)
            };

        return new PromotionApplyResult
        {
            IsSuccess = true,
            Message = "Promotion applied",
            PromotionCartItems = [
                new PromotionCartItem()
                {
                    PromotionId = promotionId,
                    Description = string.Format(Culture, "{0:C} off", discountAmount),
                    Discount = discountAmount
                }
            ]
        };
    }
}
