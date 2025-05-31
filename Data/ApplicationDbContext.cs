using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using assignment_mvc_carrental.Models;

namespace assignment_mvc_carrental.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<assignment_mvc_carrental.Models.VehicleViewModel> VehicleViewModel { get; set; } = default!;
        public DbSet<assignment_mvc_carrental.Models.CustomerViewModel> CustomerViewModel { get; set; } = default!;
        public DbSet<assignment_mvc_carrental.Models.BookingViewModel> BookingViewModel { get; set; } = default!;
    }
}
