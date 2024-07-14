using Microsoft.Extensions.Options;
using Moq;
using Promotions.Api.Data;
using Promotions.Api.Data.Models;

namespace Promotions.UnitTests.Data;
public class OptionsPromotionsRepositoryTests
{
    private readonly List<Promotion> _promotions = [];
    private readonly OptionsPromotionsRepository _sut;

    public OptionsPromotionsRepositoryTests()
    {
        var optionsMonitor = new Mock<IOptionsMonitor<IEnumerable<Promotion>>>();
        optionsMonitor
            .SetupGet(x => x.CurrentValue)
            .Returns(() => _promotions);

        _sut = new(optionsMonitor.Object);
    }

    [Fact]
    public async Task GetCurrentPromotionByCodeAsync_WhenCodeNotAvailable_ReturnsNull()
    {
        var result = await _sut.GetCurrentPromotionByCodeAsync("TEST123");
        Assert.Null(result);
    }

    [Theory]
    [InlineData("TEST123")]
    [InlineData("Test123")]
    [InlineData("test123")]
    public async Task GetCurrentPromotionByCodeAsync_WhenPromotionAvailable_ReturnsPromotion(string promotionCode)
    {
        var promotion = new Promotion
        {
            Code = "TEST123"
        };
        _promotions.Add(promotion);

        var result = await _sut.GetCurrentPromotionByCodeAsync(promotionCode);
        Assert.NotNull(result);
        Assert.Same(promotion, result);
    }
}
