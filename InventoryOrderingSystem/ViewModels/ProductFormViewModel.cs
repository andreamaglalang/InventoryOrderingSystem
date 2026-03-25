using System.ComponentModel.DataAnnotations;

namespace InventoryOrderingSystem.ViewModels
{
    public class ProductFormViewModel
    {
        public int ProductId { get; set; }

        [Required]
        public string ProductCode { get; set; } = string.Empty;

        [Required]
        public string ProductName { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
    }
}