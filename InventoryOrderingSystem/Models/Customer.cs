using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryOrderingSystem.Models
{
    [Table("Customers")]
    public partial class Customer
    {
        public int CustomerId { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}