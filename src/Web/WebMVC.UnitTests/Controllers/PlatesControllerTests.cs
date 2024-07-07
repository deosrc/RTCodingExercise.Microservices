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
        var plates = _fixture.CreateMany<Plate>(100);
        _mockCatalogService
            .Setup(x => x.GetPlatesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(plates);

        var result = await _sut.Index();

        Assert.IsType<ViewResult>(result);
        var viewResult = (ViewResult)result;
        Assert.Equal(plates, viewResult.Model);
    }
}
