using Promotions.Domain;

namespace RTCodingExercise.Microservices.Services.Promotions;

public class PromotionsApiService : IPromotionsApiService
{
    private readonly HttpClient _http;
    private readonly PromotionsApiOptions _options;
    private readonly ILogger<PromotionsApiService> _logger;

    public PromotionsApiService(HttpClient http, IOptions<PromotionsApiOptions> options, ILogger<PromotionsApiService> logger) : this(http, options.Value, logger)
    {
        // Nothing to do.
    }

    public PromotionsApiService(HttpClient http, PromotionsApiOptions options, ILogger<PromotionsApiService> logger)
    {
        _http = http;
        _options = options;
        _logger = logger;

        if (!Uri.TryCreate(_options.BaseUrl, UriKind.Absolute, out Uri? baseUri))
            throw new Exception($"Could not build base URI for {nameof(PromotionsApiService)}. Configuration value: {_options.BaseUrl}");

        _http.BaseAddress = baseUri;
    }

    public async Task<PromotionApplyResult> ApplyPromotionAsync(Cart cart, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("PromoCode/Apply", cart, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PromotionApplyResult>(cancellationToken: cancellationToken)
                ?? throw new ApiServiceException<PromotionsApiService>("Unepected response from promotions API");
        }
        catch (ApiServiceException<PromotionsApiService> ex)
        {
            _logger.LogError(ex, "Failed to apply promo code.");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to apply promo code.");
            throw new ApiServiceException<PromotionsApiService>("Failed to apply promo code.", ex);
        }
    }
}
