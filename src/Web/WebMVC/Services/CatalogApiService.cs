
namespace RTCodingExercise.Microservices.Services;

public class CatalogApiService : ICatalogService
{
    private readonly HttpClient _http;
    private readonly CatalogApiOptions _options;

    public CatalogApiService(HttpClient http, IOptions<CatalogApiOptions> options) : this(http, options.Value)
    {
        // Nothing to do.
    }

    public CatalogApiService(HttpClient http, CatalogApiOptions options)
    {
        _http = http;
        _options = options;

        if (!Uri.TryCreate(_options.BaseUrl, UriKind.Absolute, out Uri? baseUri))
            throw new Exception($"Could not build base URI for {nameof(CatalogApiService)}. Configuration value: {_options.BaseUrl}");

        _http.BaseAddress = baseUri;
    }

    public async Task<IEnumerable<Plate>> GetPlatesAsync(CancellationToken cancellationToken = default)
    {
        var result = await _http.GetFromJsonAsync<IEnumerable<Plate>>("Plates");
        return result ?? Enumerable.Empty<Plate>();
    }
}
