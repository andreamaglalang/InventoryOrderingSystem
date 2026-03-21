using InventoryOrderingSystem.Models;
using InventoryOrderingSystem.ViewModels;

namespace InventoryOrderingSystem.Services
{
    public interface IOrderService
    {
        List<Order> GetAllOrders();
        Order? GetOrderById(int id);
        CreateOrderViewModel GetOrderForEdit(int id);
        void CreateOrder(CreateOrderViewModel model);
        void UpdateOrder(CreateOrderViewModel model);
        void CompleteOrder(int orderId);
        void CancelOrder(int orderId);
    }
}