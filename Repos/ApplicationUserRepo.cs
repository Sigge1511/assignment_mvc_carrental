using assignment_mvc_carrental.Classes;
using assignment_mvc_carrental.Models;
using Microsoft.AspNetCore.Identity;

namespace assignment_mvc_carrental.Repos
{
    public class ApplicationUserRepo : IApplicationUser
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBooking _bookingRepo;

        public ApplicationUserRepo(UserManager<ApplicationUser> userManager, IBooking bookingRepo)
        {
            _userManager = userManager;
            _bookingRepo = bookingRepo;
        }


        public async Task<IdentityResult> AddCustomerAsync(CustomerViewModel customerVM)
        {
            var newUser = new ApplicationUser    //skapar newUser från CustomerViewModel
            {
                FirstName = customerVM.FirstName,    //tilldelar manuellt
                LastName = customerVM.LastName,
                Email = customerVM.Email,
                UserName = customerVM.Email,
                Address = customerVM.Address,
                City = customerVM.City,
                PhoneNumber = customerVM.PhoneNumber
            };
            var result = await _userManager.CreateAsync(newUser, customerVM.Password);
            return result;
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
