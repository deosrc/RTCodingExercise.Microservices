using Microsoft.Extensions.Options;
using Promotions.Api.Data.Models;

namespace Promotions.Api.Data;

/// <summary>
/// Basic promotions repository reading data from options.
/// </summary>
/// <remarks>
/// Only suitable for small numbers of promotions.
/// </remarks>
public class OptionsPromotionsRepository(IOptionsMonitor<IEnumerable<Promotion>> options, ILogger<OptionsPromotionsRepository> logger, TimeProvider? timeProvider = null) : IPromotionsRepository
{
    private readonly IOptionsMonitor<IEnumerable<Promotion>> _options = options;
    private readonly ILogger<OptionsPromotionsRepository> _logger = logger;
    private readonly TimeProvider _timeProvider = timeProvider ?? TimeProvider.System;

    private IEnumerable<Promotion> Promotions => _options.CurrentValue;

    public Task<Promotion?> GetCurrentPromotionByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var now = _timeProvider.GetUtcNow();

        var promotions = Promotions.Where(x => x.Code.Equals(code, StringComparison.InvariantCultureIgnoreCase)).ToList();
        _logger.LogInformation("Found {PromotionCount} promotions matching code {PromotionCode}.", promotions.Count, code);
        if (promotions.Count == 0)
            return Task.FromResult<Promotion?>(null);

        var promotion = promotions
            .Where(x => now > x.ValidFrom.ToUniversalTime())
            .Where(x => now < x.ValidUntil.ToUniversalTime())
            .SingleOrDefault();
        if (promotion is null)
        {
            _logger.LogInformation("Promotion {PromotionCode} is not within the valid period.", code);
            return Task.FromResult<Promotion?>(null);
        }

        _logger.LogInformation("Valid and current promotion found for {PromotionCode}.", code);
        return Task.FromResult(Promotions.SingleOrDefault());
    }
}
