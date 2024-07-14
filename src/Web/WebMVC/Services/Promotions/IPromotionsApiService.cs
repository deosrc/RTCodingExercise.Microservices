using Promotions.Domain;

namespace RTCodingExercise.Microservices.Services.Promotions;

public interface IPromotionsApiService
{
    Task<PromotionApplyResult> ApplyPromotionAsync(Cart cart, CancellationToken cancellationToken = default);
}
