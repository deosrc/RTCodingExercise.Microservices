using AutoFixture;
using Catalog.API.Controllers;
using Catalog.API.Data.Repositories;
using Catalog.Domain;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
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

    [Fact]
    public async Task Add_WhenAddFails_ReturnsProblem()
    {
        var plate = _fixture.Create<NewPlate>();

        _mockPlateRepository
            .Setup(x => x.AddPlateAsync(It.IsAny<NewPlate>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult<Plate>
            {
                IsSuccess = false,
                Message = "Test message"
            });

        var cancellationToken = new CancellationToken();

        // Act
        var result = await _sut.Add(plate, cancellationToken);

        // Assert
        _mockPlateRepository.Verify(x => x.AddPlateAsync(plate, cancellationToken));
        Assert.IsType<ObjectResult>(result);

        var objectResult = (ObjectResult)result;
        Assert.Equal(500, objectResult.StatusCode);
        Assert.NotNull(objectResult.Value);
        Assert.IsType<ProblemDetails>(objectResult.Value);

        var problemDetails = (ProblemDetails)objectResult.Value!;
        Assert.Equal("Operation failed", problemDetails.Title);
        Assert.Equal("Test message", problemDetails.Detail);
        Assert.Equal(500, problemDetails.Status);
    }

    [Fact]
    public async Task Add_WhenSuccessful_ReturnsPlate()
    {
        var newPlate = _fixture.Create<NewPlate>();
        var resultingPlate = _fixture.Create<Plate>();

        _mockPlateRepository
            .Setup(x => x.AddPlateAsync(It.IsAny<NewPlate>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult<Plate>
            {
                IsSuccess = true,
                Message = "Test message",
                Result = resultingPlate
            });

        var cancellationToken = new CancellationToken();

        // Act
        var result = await _sut.Add(newPlate, cancellationToken);

        // Assert
        _mockPlateRepository.Verify(x => x.AddPlateAsync(newPlate, cancellationToken));
        Assert.IsType<OkObjectResult>(result);

        var objectResult = (OkObjectResult)result;
        Assert.Equal(200, objectResult.StatusCode);
        Assert.NotNull(objectResult.Value);
        Assert.Equal(resultingPlate, objectResult.Value);
    }
}
