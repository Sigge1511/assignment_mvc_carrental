using assignment_mvc_carrental.Classes;
using Microsoft.AspNetCore.Identity;

namespace assignment_mvc_carrental.Models
{
    public class User: IdentityUser //viktigt att ärva från IdentityUser för att få användarhantering
    {
        public ICollection<Booking>? Bookings { get; set; } // Håller koll på alla bokningar som användaren har gjort
    }
}
