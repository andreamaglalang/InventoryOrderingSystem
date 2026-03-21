using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InventoryOrderingSystem.ViewModels
{
    public class CreateOrderViewModel
    {
        [Required]
        public int CustomerId { get; set; }

        public List<CreateOrderItemViewModel> Items { get; set; } = new();

        public List<SelectListItem> Customers { get; set; } = new();
        public List<SelectListItem> Products { get; set; } = new();
    }
}