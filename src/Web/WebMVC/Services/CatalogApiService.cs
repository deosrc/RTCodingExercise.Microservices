﻿using System.Text.Json;

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

    public async Task<PagedResult<Plate>> GetPlatesAsync(PagingOptions? paging = null, CancellationToken cancellationToken = default)
    {
        paging ??= new();

        try
        {
            var uri = new Uri($"Plates?Paging.Page={paging.Page}&Paging.ItemsPerPage={paging.ItemsPerPage}", UriKind.Relative);
            return await _http.GetFromJsonAsync<PagedResult<Plate>>(uri, cancellationToken: cancellationToken)
                ?? throw new ApiServiceException<CatalogApiService>("Unexpected response while retrieving plates.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve plates from Catalog API.");
            throw new ApiServiceException<CatalogApiService>("Failed to retrieve plates.", ex);
        }
    }

    public async Task<OperationResult<Plate>> AddPlateAsync(NewPlate plate, CancellationToken cancellationToken = default)
    {

        try
        {
            var response = await _http.PostAsJsonAsync("Plates", plate, cancellationToken);
            return await ParseOperationResponseAsync<Plate>(response, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add plate using Catalog API.");
            throw new ApiServiceException<CatalogApiService>("Failed to add plate.", ex);
        }
    }

    private async Task<OperationResult<TResult>> ParseOperationResponseAsync<TResult>(HttpResponseMessage response, CancellationToken cancellationToken = default)
        where TResult : class
    {
        if (response.IsSuccessStatusCode)
            return new OperationResult<TResult>
            {
                IsSuccess = true,
                Result = await response.Content.ReadFromJsonAsync<TResult>(cancellationToken: cancellationToken)
            };

        ProblemDetails? problemDetails = null;
        try
        {
            problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: cancellationToken);
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Could not parse error response: {Message}", ex.Message);
        }
        return new OperationResult<TResult>
        {
            IsSuccess = false,
            Message = problemDetails?.Detail ?? "A system error occurred. Please contact the IT team."
        };
    }
}
