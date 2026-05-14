using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using api_carrental.Dtos;
using api_carrental.Models;

namespace api_carrental.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUserDto>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<VehicleDto> VehicleSet { get; set; } = default!;
        public DbSet<BookingDto> BookingSet { get; set; } = default!;
        public DbSet<ApplicationUserDto> AppUserSet { get; set; } = default!;

        // Add entity models for internal use
        public DbSet<Vehicle> Vehicles { get; set; } = default!;
        public DbSet<Booking> Bookings { get; set; } = default!;
    }
}
