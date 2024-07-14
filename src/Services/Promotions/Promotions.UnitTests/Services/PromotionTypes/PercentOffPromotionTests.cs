using Microsoft.Extensions.Logging;
using Moq;
using Promotions.Api.Services.PromotionTypes;
using Promotions.Domain;

namespace Promotions.UnitTests.Services.PromotionTypes;
public class PercentOffPromotionTests
{
    private readonly PercentOffPromotion _sut = new(Mock.Of<ILogger<PercentOffPromotion>>());

    [Fact]
    public void TryApplyPromotion_WhenNoPercentOffOption_ThrowsException()
    {
        var cart = new Cart()
        {
            CartItems = [
                new CartItem()
                {
                    Price = 123.4M
                }
            ]
        };

        void act() => _sut.TryApplyPromotion(cart, Guid.Empty.ToString(), new Dictionary<string, string>());

        var ex = Assert.Throws<InvalidOperationException>(act);
        Assert.Equal("Promotion is not configured correctly: Missing percent off", ex.Message);
    }

    [Fact]
    public void TryApplyPromotion_WhenPercentOffOptionInvalid_ThrowsException()
    {
        var cart = new Cart()
        {
            CartItems = [
                new CartItem()
                {
                    Price = 123.4M
                }
            ]
        };

        var options = new Dictionary<string, string>
        {
            { "PercentOff", "Invalid" }
        };
        void act() => _sut.TryApplyPromotion(cart, Guid.Empty.ToString(), options);

        var ex = Assert.Throws<InvalidOperationException>(act);
        Assert.Equal("Promotion is not configured correctly: Invalid format for percent off.", ex.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(51)]
    public void TryApplyPromotion_WhenPercentOffOptionOutsideRange_ThrowsException(int percentOff)
    {
        var cart = new Cart()
        {
            CartItems = [
                new CartItem()
                {
                    Price = 123.4M
                }
            ]
        };

        var options = new Dictionary<string, string>
        {
            { "PercentOff", percentOff.ToString() }
        };
        void act() => _sut.TryApplyPromotion(cart, Guid.Empty.ToString(), options);

        var ex = Assert.Throws<InvalidOperationException>(act);
        Assert.Equal("Promotion is not configured correctly: Percent off must be between 1 and 50.", ex.Message);
    }

    [Theory]
    // Expected discount is for 2 items totaling £235.52
    [InlineData(1, 2.35, "1% off")]
    [InlineData(12, 28.26, "12% off")]
    [InlineData(15, 35.32, "15% off")]
    [InlineData(50, 117.76, "50% off")]
    public void TryApplyPromotion_WhenValid_ReturnsSuccessResult(int percentOff, decimal expectedDiscount, string expectedDescription)
    {
        var cart = new Cart()
        {
            CartItems = [
                new CartItem()
                {
                    Price = 100M
                },
                new CartItem()
                {
                    Price = 135.52M
                }
            ]
        };

        var options = new Dictionary<string, string>
        {
            { "PercentOff", percentOff.ToString() }
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
                    Discount = expectedDiscount
                }
            ]
        };
        Assert.Equivalent(expected, result);
    }
}
