using InventoryOrderingSystem.Models;
using InventoryOrderingSystem.Repositories;
using InventoryOrderingSystem.ViewModels;

namespace InventoryOrderingSystem.Services
{
    public class OrderService : IOrderService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;

        public OrderService(
            ICustomerRepository customerRepository,
            IProductRepository productRepository,
            IOrderRepository orderRepository)
        {
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
        }

        public List<Order> GetAllOrders()
        {
            return _orderRepository.GetAll();
        }

        public Order? GetOrderById(int id)
        {
            return _orderRepository.GetById(id);
        }

        public void CreateOrder(CreateOrderViewModel model)
        {
            var customer = _customerRepository.GetById(model.CustomerId);
            if (customer == null)
                throw new Exception("Customer does not exist.");

            if (!customer.IsActive)
                throw new Exception("Customer is inactive.");

            if (model.Items == null || !model.Items.Any())
                throw new Exception("At least one product is required.");

            decimal totalAmount = 0;

            var order = new Order
            {
                CustomerId = model.CustomerId,
                Status = "Pending",
                DateCreated = DateTime.Now,
                TotalAmount = 0
            };

            foreach (var item in model.Items)
            {
                var product = _productRepository.GetById(item.ProductId);
                if (product == null)
                    throw new Exception("One of the selected products does not exist.");

                if (product.Stock == 0)
                    throw new Exception($"{product.ProductName} is out of stock.");

                if (item.Quantity <= 0)
                    throw new Exception("Quantity must be greater than zero.");

                if (item.Quantity > product.Stock)
                    throw new Exception($"Insufficient stock for {product.ProductName}.");

                var lineTotal = product.Price * item.Quantity;
                totalAmount += lineTotal;

                product.Stock -= item.Quantity;
                _productRepository.Update(product);

                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price,
                    LineTotal = lineTotal
                });
            }

            order.TotalAmount = totalAmount;

            _orderRepository.Add(order);
            _orderRepository.Save();
        }

        public void CompleteOrder(int orderId)
        {
            var order = _orderRepository.GetById(orderId);
            if (order == null)
                throw new Exception("Order not found.");

            order.Status = "Completed";
            _orderRepository.Update(order);
            _orderRepository.Save();
        }

        public void CancelOrder(int orderId)
        {
            var order = _orderRepository.GetById(orderId);
            if (order == null)
                throw new Exception("Order not found.");

            order.Status = "Cancelled";
            _orderRepository.Update(order);
            _orderRepository.Save();
        }
    }
}