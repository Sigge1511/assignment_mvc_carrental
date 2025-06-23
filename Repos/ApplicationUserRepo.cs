using assignment_mvc_carrental.Classes;
using assignment_mvc_carrental.Data;
using assignment_mvc_carrental.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace assignment_mvc_carrental.Repos
{
    public class ApplicationUserRepo : IApplicationUser
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBooking _bookingRepo;
        private readonly ApplicationDbContext _context;

        public ApplicationUserRepo(UserManager<ApplicationUser> userManager, IBooking bookingRepo, ApplicationDbContext context)
        {
            _userManager = userManager;
            _bookingRepo = bookingRepo;
            _context = context;
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

        public async Task<ApplicationUser?> GetUserWithBookingsAsync(string userId)
        {
            return await _context.Users
                    .Include(u => u.Bookings!)
                        .ThenInclude(b => b.Vehicle)
                    .FirstOrDefaultAsync(u => u.Id == userId);
        }

        //Låter resten av CRUD fixas av idenitys manager direkt i controllern vilket
        //ska va ok i mindre projekt vad jag läst mig till 
    }
}
