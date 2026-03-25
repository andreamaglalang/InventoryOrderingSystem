using InventoryOrderingSystem.Data;
using InventoryOrderingSystem.Models;

namespace InventoryOrderingSystem.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly InventoryDbContext _context;

        public CustomerRepository(InventoryDbContext context)
        {
            _context = context;
        }

        public List<Customer> GetAll()
        {
            return _context.Customers.OrderBy(c => c.FullName).ToList();
        }

        public Customer? GetById(int id)
        {
            return _context.Customers.FirstOrDefault(c => c.CustomerId == id);
        }

        public Customer? GetByEmailAndPassword(string email, string password)
        {
            return _context.Customers.FirstOrDefault(c =>
                c.Email == email &&
                c.Password == password &&
                c.IsActive);
        }

        public void Add(Customer customer)
        {
            _context.Customers.Add(customer);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}