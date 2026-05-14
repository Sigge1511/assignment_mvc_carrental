using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace assignment_mvc_carrental.Models
{
    public class ApplicationUser : IdentityUser //viktigt att ärva från IdentityUser för att få användarhantering
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; } = "";
        public string? City { get; set; } = "";


        [Required]
        public override string Email { get; set; }


        public ICollection<Booking>? Bookings { get; set; } // Håller koll på alla bokningar som användaren har gjort

    }
}
