using AutoFixture;
using Catalog.Domain;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RTCodingExercise.Microservices.Controllers;
using RTCodingExercise.Microservices.Services;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace WebMVC.UnitTests.Controllers;
public class PlatesControllerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<ICatalogService> _mockCatalogService = new();
    private readonly PlatesController _sut;

    public PlatesControllerTests()
    {
        _sut = new(_mockCatalogService.Object);
    }

    [Fact]
    public async Task Index_ReturnsPlatesList()
    {
        var plates = _fixture.Create<PagedResult<Plate>>();
        _mockCatalogService
            .Setup(x => x.GetPlatesAsync(It.IsAny<PagingOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(plates);

        var result = await _sut.Index(null);

        _mockCatalogService.Verify(x => x.GetPlatesAsync(It.IsAny<PagingOptions>(), It.IsAny<CancellationToken>()), Times.Once());
        Assert.IsType<ViewResult>(result);
        var viewResult = (ViewResult)result;
        Assert.Equal(plates, viewResult.Model);
    }

    [Theory]
    [InlineData(1, 20)]
    [InlineData(2, 10)]
    [InlineData(11, 15)]
    public async Task Index_WithPageInfo_UsesPagingOptions(int page, int itemsPerPage)
    {
        var plates = _fixture.Create<PagedResult<Plate>>();
        _mockCatalogService
            .Setup(x => x.GetPlatesAsync(It.IsAny<PagingOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(plates);

        var paging = new PagingOptions
        {
            Page = page,
            ItemsPerPage = itemsPerPage
        };
        var result = await _sut.Index(paging);

        _mockCatalogService.Verify(x => x.GetPlatesAsync(It.IsAny<PagingOptions>(), It.IsAny<CancellationToken>()), Times.Once());
        _mockCatalogService.Verify(x => x.GetPlatesAsync(paging, It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task Add_WithModelError_ReturnsToPage()
    {
        var plate = _fixture.Create<NewPlate>();
        _sut.ModelState.AddModelError(string.Empty, "Model validation error");

        var result = await _sut.Add(plate);

        _mockCatalogService.Verify(x => x.GetPlatesAsync(It.IsAny<PagingOptions>(), It.IsAny<CancellationToken>()), Times.Never());
        Assert.IsType<ViewResult>(result);
        var viewResult = (ViewResult)result;
        Assert.Equal(plate, viewResult.Model);
        Assert.Null(viewResult.ViewName);
    }

    [Fact]
    public async Task Add_WhenServiceReturnsSuccessulOperation_RedirectsToList()
    {
        var plate = _fixture.Create<NewPlate>();
        _mockCatalogService
            .Setup(x => x.AddPlateAsync(It.IsAny<NewPlate>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult<Plate>()
            {
                IsSuccess = true
            });

        var result = await _sut.Add(plate);

        _mockCatalogService.Verify(x => x.AddPlateAsync(It.IsAny<NewPlate>(), It.IsAny<CancellationToken>()), Times.Once());
        _mockCatalogService.Verify(x => x.AddPlateAsync(plate, It.IsAny<CancellationToken>()), Times.Once());

        Assert.IsType<RedirectToActionResult>(result);
        var redirectResult = (RedirectToActionResult)result;
        Assert.Equal(nameof(_sut.Index), redirectResult.ActionName);
    }

    [Fact]
    public async Task Add_WhenServiceReturnsFailedOperation_ReturnsToPage()
    {
        var plate = _fixture.Create<NewPlate>();
        _mockCatalogService
            .Setup(x => x.AddPlateAsync(It.IsAny<NewPlate>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult<Plate>()
            {
                IsSuccess = false,
                Message = "Test error"
            });

        var result = await _sut.Add(plate);

        _mockCatalogService.Verify(x => x.AddPlateAsync(It.IsAny<NewPlate>(), It.IsAny<CancellationToken>()), Times.Once());
        _mockCatalogService.Verify(x => x.AddPlateAsync(plate, It.IsAny<CancellationToken>()), Times.Once());

        Assert.IsType<ViewResult>(result);
        var viewResult = (ViewResult)result;
        Assert.Equal(plate, viewResult.Model);
        Assert.Null(viewResult.ViewName);
        Assert.Collection(_sut.ModelState[string.Empty]?.Errors, x => Assert.Equal("Test error", x.ErrorMessage));
    }
}
