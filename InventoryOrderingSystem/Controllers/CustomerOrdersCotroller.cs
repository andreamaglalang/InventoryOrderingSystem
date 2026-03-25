using InventoryOrderingSystem.Data;
using InventoryOrderingSystem.Models;
using InventoryOrderingSystem.Repositories;
using InventoryOrderingSystem.Services;
using InventoryOrderingSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InventoryOrderingSystem.Controllers
{
    public class CustomerOrdersController : Controller
    {
        private readonly InventoryDbContext _context;
        private readonly IProductRepository _productRepository;
        private readonly IOrderService _orderService;

        public CustomerOrdersController(
            InventoryDbContext context,
            IProductRepository productRepository,
            IOrderService orderService)
        {
            _context = context;
            _productRepository = productRepository;
            _orderService = orderService;
        }

        private bool IsCustomerLoggedIn()
        {
            return HttpContext.Session.GetString("CustomerLoggedIn") == "true";
        }

        private int GetCustomerId()
        {
            return int.Parse(HttpContext.Session.GetString("CustomerId")!);
        }

        public IActionResult Index()
        {
            if (!IsCustomerLoggedIn())
                return RedirectToAction("Login", "CustomerAccount");

            int customerId = GetCustomerId();

            var orders = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.DateCreated)
                .ToList();

            return View(orders);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsCustomerLoggedIn())
                return RedirectToAction("Login", "CustomerAccount");

            var model = new CreateOrderViewModel
            {
                CustomerId = GetCustomerId(),
                Products = _productRepository.GetAll()
                    .Select(p => new SelectListItem
                    {
                        Value = p.ProductId.ToString(),
                        Text = $"{p.ProductCode} - {p.ProductName}"
                    }).ToList(),
                Items = new List<CreateOrderItemViewModel>
                {
                    new CreateOrderItemViewModel()
                }
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(CreateOrderViewModel model)
        {
            if (!IsCustomerLoggedIn())
                return RedirectToAction("Login", "CustomerAccount");

            model.CustomerId = GetCustomerId();

            model.Products = _productRepository.GetAll()
                .Select(p => new SelectListItem
                {
                    Value = p.ProductId.ToString(),
                    Text = $"{p.ProductCode} - {p.ProductName}"
                }).ToList();

            try
            {
                _orderService.CreateOrder(model);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        public IActionResult Details(int id)
        {
            if (!IsCustomerLoggedIn())
                return RedirectToAction("Login", "CustomerAccount");

            int customerId = GetCustomerId();

            var order = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefault(o => o.OrderId == id && o.CustomerId == customerId);

            if (order == null)
                return NotFound();

            return View(order);
        }
    }
}