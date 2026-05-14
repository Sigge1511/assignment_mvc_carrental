using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace api_carrental.Dtos
{
    public class ApplicationUserDto : IdentityUser //viktigt att ärva från IdentityUser för att få användarhantering
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; } = "";
        public string? City { get; set; } = "";

        [Required]
        public override required string? Email { get; set; }
        public ICollection<BookingDto>? Bookings { get; set; } // Håller koll på alla bokningar som användaren har gjort

        // Password property for registration (not stored in IdentityUser)
        public string? Password { get; set; }
    }
}
