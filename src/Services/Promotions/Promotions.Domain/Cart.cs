using Catalog.Domain;

namespace Promotions.Domain;

public record Cart
{
    /// <summary>
    /// The plates currently in the cart.
    /// </summary>
    public IEnumerable<Plate> Plates { get; set; } = Array.Empty<Plate>();

    /// <summary>
    /// The promotion code to apply to the cart.
    /// </summary>
    public string PromotionCode { get; set; } = string.Empty;
}
