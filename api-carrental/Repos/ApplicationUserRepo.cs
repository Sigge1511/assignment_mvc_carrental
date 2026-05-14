using api_carrental.Data;
using api_carrental.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api_carrental.Repos
{
    public class ApplicationUserRepo : IApplicationUser
    {
        private readonly UserManager<ApplicationUserDto> _userManager;
        private readonly IBookingRepo _bookingRepo;
        private readonly ApplicationDbContext _context;

        public ApplicationUserRepo(UserManager<ApplicationUserDto> userManager, IBookingRepo bookingRepo, ApplicationDbContext context)
        {
            _userManager = userManager;
            _bookingRepo = bookingRepo;
            _context = context;
        }

        public async Task<IdentityResult> AddCustomerAsync(ApplicationUserDto appUser)
        {
            try
            {
                var user = new ApplicationUserDto
                {
                    FirstName = appUser.FirstName,
                    LastName = appUser.LastName,
                    Email = appUser.Email,
                    UserName = appUser.Email,
                    PhoneNumber = appUser.PhoneNumber,
                    Address = appUser.Address,
                    City = appUser.City
                };

                // Skapa användaren med lösenord
                var result = await _userManager.CreateAsync(user, appUser.Password);

                // Om skapandet lyckades så tilldela rollen "Customer"
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Customer");
                }

                return result;
            }
            catch (Exception)
            {
                return IdentityResult.Failed(new IdentityError { Description = "An error occurred while creating the user." });
            }
        }

        public async Task<ApplicationUserDto?> GetUserByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<ApplicationUserDto?> GetUserWithBookingsAsync(string userId)
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
