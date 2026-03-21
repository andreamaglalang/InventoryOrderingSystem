using InventoryOrderingSystem.Models;

namespace InventoryOrderingSystem.Repositories
{
    public interface IOrderRepository
    {
        List<Order> GetAll();
        Order? GetById(int id);
        void Add(Order order);
        void Update(Order order);
        void Save();
    }
}