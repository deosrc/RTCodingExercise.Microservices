using Promotions.Api.Data.Models;

namespace Promotions.Api.Data;

public interface IPromotionsRepository
{
    public Task<Promotion?> GetCurrentPromotionByCodeAsync(string code, CancellationToken cancellationToken = default);
}
