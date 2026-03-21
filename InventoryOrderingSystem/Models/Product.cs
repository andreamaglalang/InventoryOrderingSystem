using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryOrderingSystem.Models
{
    [Table("Products")]
    public partial class Product
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

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}