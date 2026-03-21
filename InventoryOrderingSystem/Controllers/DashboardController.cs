using Microsoft.AspNetCore.Mvc;

namespace InventoryOrderingSystem.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") != "true")
                return RedirectToAction("Login", "Account");

            return View();
        }
    }
}