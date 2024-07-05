using AutoFixture;
using Catalog.API.Controllers;
using Catalog.API.Data.Repositories;
using Catalog.Domain;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Catalog.UnitTests.Controllers;
public class PlatesControllerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IPlateRepository> _mockPlateRepository = new();
    private readonly PlatesController _sut;

    public PlatesControllerTests()
    {
        _sut = new(_mockPlateRepository.Object);
    }

    [Fact]
    public async Task List_ReturnsAllPlates()
    {
        // Arrange
        var plates = _fixture.CreateMany<Plate>(100);
        _mockPlateRepository
            .Setup(x => x.GetPlatesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(plates);

        // Act
        var result = await _sut.List();

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var objectResult = (OkObjectResult)result;
        Assert.Equal(plates, objectResult.Value);
    }
}
