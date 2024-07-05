using AutoFixture;
using Catalog.Domain;
using RichardSzalay.MockHttp;
using RTCodingExercise.Microservices.Services;
using System.Collections;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
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
        _sut = new(new HttpClient(_mockHandler), options);
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
}
