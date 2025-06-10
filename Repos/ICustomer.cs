using assignment_mvc_carrental.Classes;

namespace assignment_mvc_carrental.Repos
{
    public interface ICustomer
    {
        Task<List<Customer>> GetAllCustomersAsync();
        Task<Customer> GetCustomerByIDAsync(int customerId);
        Task<Customer> UpdateCustomerAsync(Customer customer);
        Task DeleteCustomerAsync(Customer customer);
        Task<Customer> AddCustomerAsync(Customer customer);
    }
}
