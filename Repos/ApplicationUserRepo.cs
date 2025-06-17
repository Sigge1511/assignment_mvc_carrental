using assignment_mvc_carrental.Classes;

namespace assignment_mvc_carrental.Repos
{
    public class ApplicationUserRepo : IApplicationUser
    {
        public Task<ApplicationUserRepo> AddCustomerAsync(ApplicationUserRepo appuser)
        {
            throw new NotImplementedException();
        }

        public Task DeleteUserAsync(ApplicationUserRepo appuser)
        {
            throw new NotImplementedException();
        }

        public Task<List<ApplicationUserRepo>> GetAllUsersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationUserRepo> GetUserByIDAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationUserRepo> UpdateCustomerAsync(ApplicationUserRepo appuser)
        {
            throw new NotImplementedException();
        }
    }
}
