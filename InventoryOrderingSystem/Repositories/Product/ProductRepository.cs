using InventoryOrderingSystem.Data;
using InventoryOrderingSystem.Models;

namespace InventoryOrderingSystem.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly InventoryDbContext _context;

        public ProductRepository(InventoryDbContext context)
        {
            _context = context;
        }

        public List<Product> GetAll()
        {
            return _context.Products.OrderBy(p => p.ProductName).ToList();
        }

        public Product? GetById(int id)
        {
            return _context.Products.FirstOrDefault(p => p.ProductId == id);
        }

        public void Add(Product product)
        {
            _context.Products.Add(product);
        }

        public void Update(Product product)
        {
            _context.Products.Update(product);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}