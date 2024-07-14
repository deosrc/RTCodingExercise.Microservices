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
        var response = _fixture.Create<PagedResult<Plate>>();
        _mockHandler
            .Expect("http://localtest.me/api/Plates")
            .Respond(HttpStatusCode.OK, JsonContent.Create(response));

        var result = await _sut.GetPlatesAsync();

        _mockHandler.VerifyNoOutstandingExpectation();
        Assert.NotNull(result);
        Assert.Equal(response.Results, result!.Results);
        Assert.Equal(response.Paging, result.Paging);
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
        var newPlate = _fixture.Create<NewPlate>();
        var resultingPlate = _fixture.Create<Plate>();
        _mockHandler
            .Expect(HttpMethod.Post, "http://localtest.me/api/Plates")
            .Respond(HttpStatusCode.OK, JsonContent.Create(resultingPlate));

        var result = await _sut.AddPlateAsync(newPlate);

        var expected = new OperationResult<Plate>
        {
            IsSuccess = true,
            Result = resultingPlate
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

        var plate = _fixture.Create<NewPlate>();
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
        var plate = _fixture.Create<NewPlate>();
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
