using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryOrderingSystem.Models
{
    [Table("Orders")]
    public partial class Order
    {
        public int OrderId { get; set; }

        public int CustomerId { get; set; }

        [Required]
        public string Status { get; set; } = "Pending";

        public decimal TotalAmount { get; set; }

        public DateTime DateCreated { get; set; }

        public virtual Customer? Customer { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}