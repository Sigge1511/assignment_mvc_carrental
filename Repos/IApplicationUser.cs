using assignment_mvc_carrental.Classes;
using assignment_mvc_carrental.Models;
using Microsoft.AspNetCore.Identity;

namespace assignment_mvc_carrental.Repos
{
    public interface IApplicationUser
    {
        Task<IdentityResult> AddCustomerAsync(CustomerViewModel customerVM);

        Task<ApplicationUser?> GetUserWithBookingsAsync(string userId);

        //låter resten av CRUD fixas av idenitys manager direkt i controllern
    }
}