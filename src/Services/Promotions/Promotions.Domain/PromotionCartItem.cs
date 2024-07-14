namespace Promotions.Domain;

public record PromotionCartItem
{
    /// <summary>
    /// The ID of the promotion.
    /// </summary>
    public string PromotionId { get; set; } = string.Empty;

    /// <summary>
    /// The description of the promotion to displat to the user.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The discount amount applied.
    /// </summary>
    public decimal Discount { get; set; }
}
