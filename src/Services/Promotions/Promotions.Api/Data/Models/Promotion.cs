namespace Promotions.Api.Data.Models;

public record Promotion
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime ValidFrom { get; set; }
    public DateTime ValidUntil { get; set; }
    public IReadOnlyDictionary<string, string> Options { get; set; } = new Dictionary<string, string>();
}
