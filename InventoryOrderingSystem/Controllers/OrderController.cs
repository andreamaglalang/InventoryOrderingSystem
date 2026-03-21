using InventoryOrderingSystem.Repositories;
using InventoryOrderingSystem.Services;
using InventoryOrderingSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryOrderingSystem.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;

        public OrdersController(
            IOrderService orderService,
            ICustomerRepository customerRepository,
            IProductRepository productRepository)
        {
            _orderService = orderService;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
        }

        private bool IsAdminLoggedIn()
        {
            return HttpContext.Session.GetString("AdminLoggedIn") == "true";
        }

        public IActionResult Index()
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Account");

            var orders = _orderService.GetAllOrders();
            return View(orders);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Account");

            var model = new CreateOrderViewModel
            {
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
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Account");

            ReloadDropdowns(model);

            if (!ModelState.IsValid)
                return View(model);

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

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Account");

            try
            {
                var model = _orderService.GetOrderForEdit(id);
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public IActionResult Edit(CreateOrderViewModel model)
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Account");

            ReloadDropdowns(model);

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                _orderService.UpdateOrder(model);
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
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Account");

            var order = _orderService.GetOrderById(id);
            if (order == null)
                return NotFound();

            return View(order);
        }

        [HttpPost]
        public IActionResult Complete(int id)
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Account");

            _orderService.CompleteOrder(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Cancel(int id)
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Account");

            _orderService.CancelOrder(id);
            return RedirectToAction(nameof(Index));
        }

        private void ReloadDropdowns(CreateOrderViewModel model)
        {
            model.Customers = _customerRepository.GetAll()
                .Where(c => c.IsActive)
                .Select(c => new SelectListItem
                {
                    Value = c.CustomerId.ToString(),
                    Text = c.FullName
                }).ToList();

            model.Products = _productRepository.GetAll()
                .Select(p => new SelectListItem
                {
                    Value = p.ProductId.ToString(),
                    Text = $"{p.ProductCode} - {p.ProductName}"
                }).ToList();
        }
    }
}