using InventoryOrderingSystem.Models;

namespace InventoryOrderingSystem.Repositories
{
    public interface IProductRepository
    {
        List<Product> GetAll();
        Product? GetById(int id);
        void Add(Product product);
        void Update(Product product);
        void Save();
    }
}