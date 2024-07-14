using Catalog.Domain;
using Microsoft.Extensions.Logging;
using Moq;
using Promotions.Api.Services.PromotionTypes;
using Promotions.Domain;

namespace Promotions.UnitTests.Services.PromotionTypes;
public class MoneyOffPromotionTests
{
    private readonly MoneyOffPromotion _sut = new(Mock.Of<ILogger<MoneyOffPromotion>>());

    [Fact]
    public void TryApplyPromotion_WhenNoDiscountOption_ThrowsException()
    {
        var cart = new Cart()
        {
            Plates = [
                new Plate()
                {
                    SalePrice = 123.4M
                }
            ]
        };

        void act() => _sut.TryApplyPromotion(cart, Guid.Empty.ToString(), new Dictionary<string, string>());

        var ex = Assert.Throws<InvalidOperationException>(act);
        Assert.Equal("Promotion is not configured correctly: Missing discount amount", ex.Message);
    }

    [Fact]
    public void TryApplyPromotion_WhenDiscountOptionInvalid_ThrowsException()
    {
        var cart = new Cart()
        {
            Plates = [
                new Plate()
                {
                    SalePrice = 123.4M
                }
            ]
        };

        var options = new Dictionary<string, string>
        {
            { "DiscountAmount", "Invalid" }
        };
        void act() => _sut.TryApplyPromotion(cart, Guid.Empty.ToString(), options);

        var ex = Assert.Throws<InvalidOperationException>(act);
        Assert.Equal("Promotion is not configured correctly: Invalid format for discount amount.", ex.Message);
    }

    [Theory]
    [InlineData(10.0, 10.0, 22.3, "Promotion requires a cart total of at least £22.30.")]
    [InlineData(10.0, 12.2, 22.3, "Promotion requires a cart total of at least £22.30.")]
    [InlineData(10.0, 13.2, 23.3, "Promotion requires a cart total of at least £23.30.")]
    public void TryApplyPromotion_WhenCartValueLessThenDiscount_ReturnsFailureResult(decimal item1Amount, decimal item2Amount, decimal discountAmount, string expectedDescription)
    {
        var cart = new Cart()
        {
            Plates = [
                new Plate()
                {
                    SalePrice = item1Amount
                },
                new Plate()
                {
                    SalePrice = item2Amount
                }
            ]
        };

        var options = new Dictionary<string, string>
        {
            { "DiscountAmount", discountAmount.ToString() }
        };
        var promotionId = Guid.NewGuid().ToString();
        var result = _sut.TryApplyPromotion(cart, promotionId, options);

        var expected = new PromotionApplyResult
        {
            IsSuccess = false,
            Message = expectedDescription,
            PromotionCartItems = []
        };
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(10.0, 12.3, 22.3, "£22.30 off")]
    [InlineData(10.0, 13.3, 23.3, "£23.30 off")]
    [InlineData(50.0, 5.3, 23.3, "£23.30 off")]
    public void TryApplyPromotion_WhenCartValueMoreThenDiscount_ReturnsSuccessResult(decimal item1Amount, decimal item2Amount, decimal discountAmount, string expectedDescription)
    {
        var cart = new Cart()
        {
            Plates = [
                new Plate()
                {
                    SalePrice = item1Amount
                },
                new Plate()
                {
                    SalePrice = item2Amount
                }
            ]
        };

        var options = new Dictionary<string, string>
        {
            { "DiscountAmount", discountAmount.ToString() }
        };
        var promotionId = Guid.NewGuid().ToString();
        var result = _sut.TryApplyPromotion(cart, promotionId, options);

        var expected = new PromotionApplyResult
        {
            IsSuccess = true,
            Message = "Promotion applied",
            PromotionCartItems = [
                new PromotionCartItem()
                {
                    PromotionId = promotionId.ToString(),
                    Description = expectedDescription,
                    Discount = discountAmount
                }
            ]
        };
        Assert.Equivalent(expected, result);
    }
}
