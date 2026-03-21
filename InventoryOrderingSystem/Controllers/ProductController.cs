using InventoryOrderingSystem.Models;
using InventoryOrderingSystem.Repositories;
using InventoryOrderingSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace InventoryOrderingSystem.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
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

            var products = _productRepository.GetAll();
            return View(products);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Account");

            return View(new ProductFormViewModel());
        }

        [HttpPost]
        public IActionResult Create(ProductFormViewModel model)
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            var product = new Product
            {
                ProductCode = model.ProductCode,
                ProductName = model.ProductName,
                Stock = model.Stock,
                Price = model.Price
            };

            _productRepository.Add(product);
            _productRepository.Save();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Account");

            var product = _productRepository.GetById(id);
            if (product == null)
                return NotFound();

            var model = new ProductFormViewModel
            {
                ProductId = product.ProductId,
                ProductCode = product.ProductCode,
                ProductName = product.ProductName,
                Stock = product.Stock,
                Price = product.Price
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(ProductFormViewModel model)
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            var product = _productRepository.GetById(model.ProductId);
            if (product == null)
                return NotFound();

            product.ProductCode = model.ProductCode;
            product.ProductName = model.ProductName;
            product.Stock = model.Stock;
            product.Price = model.Price;

            _productRepository.Update(product);
            _productRepository.Save();

            return RedirectToAction(nameof(Index));
        }
    }
}