using System.ComponentModel.DataAnnotations;

namespace InventoryOrderingSystem.ViewModels
{
    public class CustomerFormViewModel
    {
        public int CustomerId { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; } = true;
    }
}