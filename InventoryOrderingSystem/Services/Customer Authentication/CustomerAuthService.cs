using InventoryOrderingSystem.Models;
using InventoryOrderingSystem.Repositories;

namespace InventoryOrderingSystem.Services
{
    public class CustomerAuthService : ICustomerAuthService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerAuthService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public Customer? ValidateCustomer(string email, string password)
        {
            return _customerRepository.GetByEmailAndPassword(email, password);
        }
    }
}