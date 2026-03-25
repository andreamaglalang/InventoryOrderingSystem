using InventoryOrderingSystem.Services;
using InventoryOrderingSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace InventoryOrderingSystem.Controllers
{
    public class CustomerAccountController : Controller
    {
        private readonly ICustomerAuthService _customerAuthService;

        public CustomerAccountController(ICustomerAuthService customerAuthService)
        {
            _customerAuthService = customerAuthService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(CustomerLoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var customer = _customerAuthService.ValidateCustomer(model.Email, model.Password);

            if (customer == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

            HttpContext.Session.SetString("CustomerLoggedIn", "true");
            HttpContext.Session.SetString("CustomerId", customer.CustomerId.ToString());
            HttpContext.Session.SetString("CustomerName", customer.FullName);

            return RedirectToAction("Index", "CustomerOrders");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("CustomerLoggedIn");
            HttpContext.Session.Remove("CustomerId");
            HttpContext.Session.Remove("CustomerName");

            return RedirectToAction("Login");
        }
    }
}