
namespace RTCodingExercise.Microservices.Services;

public class CatalogApiService : ICatalogService
{
    private readonly HttpClient _http;
    private readonly CatalogApiOptions _options;
    private readonly ILogger<CatalogApiService> _logger;

    public CatalogApiService(HttpClient http, IOptions<CatalogApiOptions> options, ILogger<CatalogApiService> logger) : this(http, options.Value, logger)
    {
        // Nothing to do.
    }

    public CatalogApiService(HttpClient http, CatalogApiOptions options, ILogger<CatalogApiService> logger)
    {
        _http = http;
        _options = options;
        _logger = logger;

        if (!Uri.TryCreate(_options.BaseUrl, UriKind.Absolute, out Uri? baseUri))
            throw new Exception($"Could not build base URI for {nameof(CatalogApiService)}. Configuration value: {_options.BaseUrl}");

        _http.BaseAddress = baseUri;
    }

    public async Task<IEnumerable<Plate>> GetPlatesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _http.GetFromJsonAsync<IEnumerable<Plate>>("Plates");
            return result ?? Enumerable.Empty<Plate>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve plates from Catalog API.");
            throw new ApiServiceException<CatalogApiService>("Failed to retrieve plates.", ex);
        }
    }
}
