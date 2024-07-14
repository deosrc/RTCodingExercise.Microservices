using System.ComponentModel.DataAnnotations;

namespace Catalog.Domain
{
    public record NewPlate
    {
        [Required]
        [RegularExpression("[A-Z0-9]{1,7}")]
        public string? Registration { get; set; }

        [Required]
        public decimal PurchasePrice { get; set; }

        [Required]
        public decimal SalePrice { get; set; }

        [Required]
        [RegularExpression("[A-Z]{1,7}")]
        public string? Letters { get; set; }

        [Required]
        public int Numbers { get; set; }
    }
}