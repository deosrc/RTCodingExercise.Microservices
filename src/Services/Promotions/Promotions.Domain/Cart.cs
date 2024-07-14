namespace Promotions.Domain;

public record Cart
{
    /// <summary>
    /// The plates currently in the cart.
    /// </summary>
    public IEnumerable<CartItem> CartItems { get; set; } = Array.Empty<CartItem>();

    /// <summary>
    /// The promotion code to apply to the cart.
    /// </summary>
    public string PromotionCode { get; set; } = string.Empty;
}
