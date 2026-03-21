namespace InventoryOrderingSystem.Services
{
    public interface IAuthService
    {
        bool ValidateAdmin(string username, string password);
    }
}