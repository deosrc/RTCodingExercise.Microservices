using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using Promotions.Api.Data;
using Promotions.Api.Data.Models;
using Promotions.Api.Services;
using Promotions.Domain;

namespace Promotions.UnitTests.Services;
public class CartAdjustmentServiceTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IPromotionsRepository> _mockPromotionRepository = new();
    private readonly CartAdjustmentService _sut;

    public CartAdjustmentServiceTests()
    {
        _sut = new(_mockPromotionRepository.Object, Mock.Of<ILogger<CartAdjustmentService>>());
    }

    [Fact]
    public async Task TryApplyPromotionAsync_WhenCodeInvalid_ReturnsFalse()
    {
        _mockPromotionRepository
            .Setup(x => x.GetCurrentPromotionByCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<Promotion?>(null));

        var cart = _fixture
            .Build<Cart>()
            .With(x => x.PromotionCode, "TEST123")
            .Create();

        var result = await _sut.TryApplyPromotionAsync(cart);

        var expected = new PromotionApplyResult()
        {
            IsSuccess = false,
            Message = "Promotion code is not valid."
        };
        Assert.Multiple(
            () => _mockPromotionRepository.Verify(x => x.GetCurrentPromotionByCodeAsync("TEST123", It.IsAny<CancellationToken>())),
            () => Assert.Equal(expected, result)
        );
    }
}
