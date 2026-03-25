using InventoryOrderingSystem.Models;

namespace InventoryOrderingSystem.Repositories
{
    public interface ICustomerRepository
    {
        List<Customer> GetAll();
        Customer? GetById(int id);
        Customer? GetByEmailAndPassword(string email, string password);
        void Add(Customer customer);
        void Save();
    }
}