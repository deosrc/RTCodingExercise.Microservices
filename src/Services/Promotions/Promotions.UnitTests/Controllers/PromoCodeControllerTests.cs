using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Promotions.Api.Controllers;
using Promotions.Api.Services;
using Promotions.Domain;

namespace Promotions.UnitTests.Controllers;
public class PromoCodeControllerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<ICartAdjustmentService> _mockPromotionService = new();
    private readonly PromoCodeController _sut;

    public PromoCodeControllerTests()
    {
        _sut = new(_mockPromotionService.Object, Mock.Of<ILogger<PromoCodeController>>());
    }

    [Fact]
    public async Task Apply_WhenFailed_ReturnsOkWithInformation()
    {
        _mockPromotionService
            .Setup(x => x.TryApplyPromotionAsync(It.IsAny<Cart>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PromotionApplyResult()
            {
                IsSuccess = false,
                Message = "Test message"
            });

        var cart = _fixture.Create<Cart>();
        var result = await _sut.Apply(cart);

        _mockPromotionService.Verify(x => x.TryApplyPromotionAsync(It.IsAny<Cart>(), It.IsAny<CancellationToken>()), Times.Once());
        _mockPromotionService.Verify(x => x.TryApplyPromotionAsync(cart, It.IsAny<CancellationToken>()), Times.Once());

        Assert.IsType<OkObjectResult>(result);
        var objectResult = (OkObjectResult)result;
        Assert.NotNull(objectResult.Value);

        var expected = new PromotionApplyResult()
        {
            IsSuccess = false,
            Message = "Test message"
        };
        Assert.Equal(expected, objectResult.Value);
    }

    [Fact]
    public async Task Apply_WhenCodeValid_ReturnsPromotion()
    {
        var expected = new PromotionApplyResult()
        {
            IsSuccess = true,
            Message = "Test message",
            PromotionCartItems = [
                new PromotionCartItem()
                {
                    PromotionId = "TEST",
                    Description = "Test Promotion",
                    Discount = 12.3M
                }
            ]
        };
        _mockPromotionService
            .Setup(x => x.TryApplyPromotionAsync(It.IsAny<Cart>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var cart = _fixture.Create<Cart>();
        var result = await _sut.Apply(cart);

        _mockPromotionService.Verify(x => x.TryApplyPromotionAsync(It.IsAny<Cart>(), It.IsAny<CancellationToken>()), Times.Once());
        _mockPromotionService.Verify(x => x.TryApplyPromotionAsync(cart, It.IsAny<CancellationToken>()), Times.Once());

        Assert.IsType<OkObjectResult>(result);
        var objectResult = (OkObjectResult)result;
        Assert.NotNull(objectResult.Value);
        Assert.Same(expected, objectResult.Value);
    }
}
