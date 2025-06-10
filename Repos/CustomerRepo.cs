using assignment_mvc_carrental.Classes;

namespace assignment_mvc_carrental.Repos
{
    public class CustomerRepo : ICustomer
    {
        public Task<Customer> AddCustomerAsync(Customer customer)
        {
            throw new NotImplementedException();
        }
        //*************************************************************************************************

        public Task DeleteCustomerAsync(Customer customer)
        {
            throw new NotImplementedException();
        }
        //*************************************************************************************************

        public Task<List<Customer>> GetAllCustomersAsync()
        {
            throw new NotImplementedException();
        }
        //*************************************************************************************************

        public Task<Customer> GetCustomerByIDAsync(int customerId)
        {
            throw new NotImplementedException();
        }
        //*************************************************************************************************

        public Task<Customer> UpdateCustomerAsync(Customer customer)
        {
            throw new NotImplementedException();
        }
    }
}
