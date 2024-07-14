namespace Promotions.Domain;

public record CartItem
{
    public Guid PlateId { get; set; }
    public decimal Price { get; set; }
}