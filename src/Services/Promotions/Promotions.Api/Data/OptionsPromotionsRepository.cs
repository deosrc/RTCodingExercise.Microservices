using Microsoft.Extensions.Options;
using Promotions.Api.Data.Models;

namespace Promotions.Api.Data;

/// <summary>
/// Basic promotions repository reading data from options.
/// </summary>
/// <remarks>
/// Only suitable for small numbers of promotions.
/// </remarks>
public class OptionsPromotionsRepository(IOptionsMonitor<IEnumerable<Promotion>> options, TimeProvider timeProvider = null) : IPromotionsRepository
{
    private readonly IOptionsMonitor<IEnumerable<Promotion>> _options = options;
    private readonly TimeProvider _timeProvider = timeProvider ?? TimeProvider.System;

    private IEnumerable<Promotion> Promotions => _options.CurrentValue;

    public Task<Promotion?> GetCurrentPromotionByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Promotions.SingleOrDefault(x => x.Code.Equals(code, StringComparison.InvariantCultureIgnoreCase)));
    }
}
