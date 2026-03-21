using InventoryOrderingSystem.Models;
using InventoryOrderingSystem.Repositories;
using InventoryOrderingSystem.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        public CreateOrderViewModel GetOrderForEdit(int id)
        {
            var order = _orderRepository.GetById(id);
            if (order == null)
                throw new Exception("Order not found.");

            return new CreateOrderViewModel
            {
                OrderId = order.OrderId,
                CustomerId = order.CustomerId,
                Items = order.OrderItems.Select(i => new CreateOrderItemViewModel
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList(),
                Customers = _customerRepository.GetAll()
                    .Where(c => c.IsActive)
                    .Select(c => new SelectListItem
                    {
                        Value = c.CustomerId.ToString(),
                        Text = c.FullName
                    }).ToList(),
                Products = _productRepository.GetAll()
                    .Select(p => new SelectListItem
                    {
                        Value = p.ProductId.ToString(),
                        Text = $"{p.ProductCode} - {p.ProductName}"
                    }).ToList()
            };
        }

        public void CreateOrder(CreateOrderViewModel model)
        {
            ValidateOrderModel(model);

            var customer = _customerRepository.GetById(model.CustomerId)!;

            var order = new Order
            {
                CustomerId = model.CustomerId,
                Status = "Pending",
                DateCreated = DateTime.Now,
                TotalAmount = 0
            };

            decimal totalAmount = 0;

            foreach (var item in model.Items)
            {
                var product = _productRepository.GetById(item.ProductId);
                if (product == null)
                    throw new Exception("One of the selected products does not exist.");

                if (product.Stock == 0)
                    throw new Exception($"{product.ProductName} is out of stock.");

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

        public void UpdateOrder(CreateOrderViewModel model)
        {
            ValidateOrderModel(model);

            var order = _orderRepository.GetById(model.OrderId);
            if (order == null)
                throw new Exception("Order not found.");

            if (order.Status == "Completed")
                throw new Exception("Completed orders cannot be modified.");

            var customer = _customerRepository.GetById(model.CustomerId);
            if (customer == null)
                throw new Exception("Customer does not exist.");

            if (!customer.IsActive)
                throw new Exception("Customer is inactive.");

            // Restore stock from old items
            foreach (var oldItem in order.OrderItems)
            {
                var oldProduct = _productRepository.GetById(oldItem.ProductId);
                if (oldProduct != null)
                {
                    oldProduct.Stock += oldItem.Quantity;
                    _productRepository.Update(oldProduct);
                }
            }

            // Remove old items
            _orderRepository.RemoveOrderItems(order.OrderItems.ToList());
            order.OrderItems.Clear();

            // Apply new items
            decimal totalAmount = 0;

            foreach (var item in model.Items)
            {
                var product = _productRepository.GetById(item.ProductId);
                if (product == null)
                    throw new Exception("One of the selected products does not exist.");

                if (product.Stock == 0)
                    throw new Exception($"{product.ProductName} is out of stock.");

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

            order.CustomerId = model.CustomerId;
            order.TotalAmount = totalAmount;

            _orderRepository.Update(order);
            _orderRepository.Save();
        }

        public void CompleteOrder(int orderId)
        {
            var order = _orderRepository.GetById(orderId);
            if (order == null)
                throw new Exception("Order not found.");

            if (order.Status == "Cancelled")
                throw new Exception("Cancelled orders cannot be completed.");

            order.Status = "Completed";
            _orderRepository.Update(order);
            _orderRepository.Save();
        }

        public void CancelOrder(int orderId)
        {
            var order = _orderRepository.GetById(orderId);
            if (order == null)
                throw new Exception("Order not found.");

            if (order.Status == "Completed")
                throw new Exception("Completed orders cannot be cancelled.");

            // Restore stock when cancelling
            foreach (var item in order.OrderItems)
            {
                var product = _productRepository.GetById(item.ProductId);
                if (product != null)
                {
                    product.Stock += item.Quantity;
                    _productRepository.Update(product);
                }
            }

            order.Status = "Cancelled";
            _orderRepository.Update(order);
            _orderRepository.Save();
        }

        private void ValidateOrderModel(CreateOrderViewModel model)
        {
            var customer = _customerRepository.GetById(model.CustomerId);
            if (customer == null)
                throw new Exception("Customer does not exist.");

            if (!customer.IsActive)
                throw new Exception("Customer is inactive.");

            if (model.Items == null || !model.Items.Any())
                throw new Exception("At least one product is required.");

            if (model.Items.Any(i => i.ProductId <= 0))
                throw new Exception("Please select a product for every order row.");

            if (model.Items.Any(i => i.Quantity <= 0))
                throw new Exception("Quantity must be greater than zero.");

            var duplicateProducts = model.Items
                .GroupBy(i => i.ProductId)
                .Any(g => g.Count() > 1);

            if (duplicateProducts)
                throw new Exception("Duplicate products are not allowed in the same order.");
        }
    }
}