using InventoryOrderingSystem.Models;
using InventoryOrderingSystem.Repositories;
using InventoryOrderingSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace InventoryOrderingSystem.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomersController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        private bool IsAdminLoggedIn()
        {
            return HttpContext.Session.GetString("AdminLoggedIn") == "true";
        }

        public IActionResult Index()
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Account");

            var customers = _customerRepository.GetAll();
            return View(customers);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Account");

            return View(new CustomerFormViewModel());
        }

        [HttpPost]
        public IActionResult Create(CustomerFormViewModel model)
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            var customer = new Customer
            {
                FullName = model.FullName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                IsActive = model.IsActive
            };

            _customerRepository.Add(customer);
            _customerRepository.Save();

            return RedirectToAction(nameof(Index));
        }
    }
}