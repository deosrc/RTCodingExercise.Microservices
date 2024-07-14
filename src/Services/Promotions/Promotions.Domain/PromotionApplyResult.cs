namespace Promotions.Domain;
public record PromotionApplyResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public IEnumerable<PromotionCartItem> PromotionCartItems { get; set; } = Array.Empty<PromotionCartItem>();
}
