using AutoFixture;
using Catalog.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using RichardSzalay.MockHttp;
using RTCodingExercise.Microservices.Services;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace WebMVC.UnitTests.Services;
public class CatalogApiServiceTests
{
    private readonly Fixture _fixture = new();
    private readonly MockHttpMessageHandler _mockHandler = new MockHttpMessageHandler();
    private readonly CatalogApiService _sut;

    public CatalogApiServiceTests()
    {
        var options = new CatalogApiOptions
        {
            BaseUrl = "http://localtest.me/api/"
        };
        _sut = new(new HttpClient(_mockHandler), options, Mock.Of<ILogger<CatalogApiService>>());
    }

    [Fact]
    public async Task GetPlatesAsync_WhenSuccessResponse_ReturnsPlates()
    {
        var plates = _fixture.CreateMany<Plate>(100);
        _mockHandler
            .Expect("http://localtest.me/api/Plates")
            .Respond(HttpStatusCode.OK, JsonContent.Create(plates));

        var result = await _sut.GetPlatesAsync();

        _mockHandler.VerifyNoOutstandingExpectation();
        Assert.Equal(plates, result);
    }

    [Fact]
    public async Task GetPlatesAsync_WhenErrorResponse_ThrowsException()
    {
        _mockHandler
            .Expect("http://localtest.me/api/Plates")
            .Respond(HttpStatusCode.InternalServerError);

        async Task act() => await _sut.GetPlatesAsync();

        var ex = await Assert.ThrowsAsync<ApiServiceException<CatalogApiService>>(act);
    }

    [Fact]
    public async Task AddPlateAsync_WhenSuccessResponse_ReturnsSuccessOperationResult()
    {
        var plate = _fixture.Create<Plate>();
        _mockHandler
            .Expect(HttpMethod.Post, "http://localtest.me/api/Plates")
            .Respond(HttpStatusCode.OK, JsonContent.Create(plate));

        var result = await _sut.AddPlateAsync(plate);

        var expected = new OperationResult<Plate>
        {
            IsSuccess = true,
            Result = plate
        };

        _mockHandler.VerifyNoOutstandingExpectation();
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task AddPlateAsync_WhenProblemDetailsErrorResponse_ThrowsException()
    {
        var problemDetails = new ProblemDetails
        {
            Detail = "Test error"
        };
        _mockHandler
            .Expect(HttpMethod.Post, "http://localtest.me/api/Plates")
            .Respond(HttpStatusCode.BadRequest, JsonContent.Create(problemDetails));

        var plate = _fixture.Create<Plate>();
        var result = await _sut.AddPlateAsync(plate);

        var expected = new OperationResult<Plate>
        {
            IsSuccess = false,
            Message = "Test error"
        };

        _mockHandler.VerifyNoOutstandingExpectation();
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task AddPlateAsync_WhenErrorResponse_ReturnsFailedOperationResult()
    {
        var plate = _fixture.Create<Plate>();
        _mockHandler
            .Expect(HttpMethod.Post, "http://localtest.me/api/Plates")
            .Respond(HttpStatusCode.InternalServerError);

        var result = await _sut.AddPlateAsync(plate);

        var expected = new OperationResult<Plate>
        {
            IsSuccess = false,
            Message = "A system error occurred. Please contact the IT team."
        };

        _mockHandler.VerifyNoOutstandingExpectation();
        Assert.Equal(expected, result);
    }
}
