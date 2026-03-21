using InventoryOrderingSystem.Data;
using InventoryOrderingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryOrderingSystem.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly InventoryDbContext _context;

        public OrderRepository(InventoryDbContext context)
        {
            _context = context;
        }

        public List<Order> GetAll()
        {
            return _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.DateCreated)
                .ToList();
        }

        public Order? GetById(int id)
        {
            return _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefault(o => o.OrderId == id);
        }

        public void Add(Order order)
        {
            _context.Orders.Add(order);
        }

        public void Update(Order order)
        {
            _context.Orders.Update(order);
        }

        public void RemoveOrderItems(IEnumerable<OrderItem> items)
        {
            _context.OrderItems.RemoveRange(items);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}