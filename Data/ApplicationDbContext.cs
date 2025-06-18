using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using assignment_mvc_carrental.Classes;

namespace assignment_mvc_carrental.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Vehicle> VehicleSet { get; set; } = default!;
        public DbSet<Booking> BookingSet { get; set; } = default!;
        public DbSet<ApplicationUser> AppUserSet { get; set; } = default!;

    }
}
