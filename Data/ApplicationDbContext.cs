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
        public DbSet<assignment_mvc_carrental.Classes.Vehicle> VehicleSet { get; set; } = default!;
        public DbSet<assignment_mvc_carrental.Classes.Customer> CustomerSet { get; set; } = default!;
        public DbSet<assignment_mvc_carrental.Classes.Booking> BookingSet { get; set; } = default!;
        public DbSet<assignment_mvc_carrental.Classes.UserRole> UserRoleSet { get; set; } = default!;
        public DbSet<assignment_mvc_carrental.Classes.Admin> AdminSet { get; set; } = default!;
    }
}
