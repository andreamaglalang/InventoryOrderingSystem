using InventoryOrderingSystem.Models;

namespace InventoryOrderingSystem.Services
{
    public interface ICustomerAuthService
    {
        Customer? ValidateCustomer(string email, string password);
    }
}