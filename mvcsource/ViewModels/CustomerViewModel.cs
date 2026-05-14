using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace assignment_mvc_carrental.ViewModels
{
    public class CustomerViewModel
    {
        public string? Id { get; set; }


        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }


        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }


        [Required(ErrorMessage = "Full address is required")]
        public string Address { get; set; }


        [Required(ErrorMessage = "Full address is required")]
        public string City { get; set; }

        [Required(ErrorMessage = "Phonenumber is required")]
        public string PhoneNumber { get; set; }


        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }


        [BindNever] //hjälper till med strul vid edit av kunder
        public string? Password { get; set; } // Endast för registreringen



        public List<BookingViewModel> Bookings { get; set; } = new();


    }
}
