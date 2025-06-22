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


        public async Task<IdentityResult> AddCustomerAsync(CustomerViewModel viewModel)
        {
            var user = new ApplicationUser
            {
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                Email = viewModel.Email,
                UserName = viewModel.Email,
                PhoneNumber = viewModel.PhoneNumber,
                Address = viewModel.Address,
                City = viewModel.City
            };

            // Skapa användaren med lösenord
            var result = await _userManager.CreateAsync(user, viewModel.Password);

            // Om skapandet lyckades så tilldela rollen "Customer"
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Customer");
            }

            return result;
        }

        //Låter resten av CRUD fixas av idenitys manager direkt i controllern vilket
        //ska va ok i mindre projekt vad jag läst mig till 
    }
}
