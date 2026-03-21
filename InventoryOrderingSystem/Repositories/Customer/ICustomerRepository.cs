using InventoryOrderingSystem.Models;

namespace InventoryOrderingSystem.Repositories
{
    public interface ICustomerRepository
    {
        List<Customer> GetAll();
        Customer? GetById(int id);
        void Add(Customer customer);
        void Save();
    }
}