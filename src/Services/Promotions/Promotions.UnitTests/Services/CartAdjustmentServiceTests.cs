using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using Promotions.Api.Data;
using Promotions.Api.Data.Models;
using Promotions.Api.Services;
using Promotions.Api.Services.PromotionTypes;
using Promotions.Domain;

namespace Promotions.UnitTests.Services;
public class CartAdjustmentServiceTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IPromotionsRepository> _mockPromotionRepository = new();
    private readonly Mock<IMoneyOffPromotion> _mockMoneyOffPromotion = new();
    private readonly CartAdjustmentService _sut;

    public CartAdjustmentServiceTests()
    {
        _sut = new(_mockPromotionRepository.Object, _mockMoneyOffPromotion.Object, Mock.Of<ILogger<CartAdjustmentService>>());
    }

    [Fact]
    public async Task TryApplyPromotionAsync_WhenCodeInvalid_ReturnsFailureResult()
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

    [Fact]
    public async Task TryApplyPromotionAsync_WhenUnknownPromotionType_ThrowsException()
    {
        _mockPromotionRepository
            .Setup(x => x.GetCurrentPromotionByCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Promotion()
            {
                Id = Guid.Parse("64f3d985-f036-444e-b6ee-a38b239d53b3"),
                Description = "Test promotion",
                Code = "TEST123",
                Type = 0
            });

        var cart = _fixture
            .Build<Cart>()
            .With(x => x.PromotionCode, "TEST123")
            .Create();

        async Task act() => await _sut.TryApplyPromotionAsync(cart);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(act);
        Assert.Equal("Unrecognised promotion type.", ex.Message);
    }

    [Fact]
    public async Task TryApplyPromotionAsync_WhenMoneyOffPromotionType_ReturnsPromotionResult()
    {
        _mockPromotionRepository
            .Setup(x => x.GetCurrentPromotionByCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Promotion()
            {
                Id = Guid.Parse("64f3d985-f036-444e-b6ee-a38b239d53b3"),
                Description = "Test promotion",
                Code = "TEST123",
                Type = PromotionType.MoneyOff
            });

        var expected = _fixture
            .Build<PromotionApplyResult>()
            .With(x => x.IsSuccess, true)
            .Create();
        _mockMoneyOffPromotion
            .Setup(x => x.TryApplyPromotion(It.IsAny<Cart>(), It.IsAny<IReadOnlyDictionary<string, string>>()))
            .Returns(expected);

        var cart = _fixture
            .Build<Cart>()
            .With(x => x.PromotionCode, "TEST123")
            .Create();

        var result = await _sut.TryApplyPromotionAsync(cart);

        Assert.Same(expected, result);
    }
}
