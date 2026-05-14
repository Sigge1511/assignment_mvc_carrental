using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace api_carrental.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; } = "";
        public string? City { get; set; } = "";

        [Required]
        public override string? Email { get; set; }

        public ICollection<Booking>? Bookings { get; set; }
    }
}