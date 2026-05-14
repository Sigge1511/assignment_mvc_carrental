using System.ComponentModel.DataAnnotations;

namespace api_carrental.Dtos
{
    public class UserRegistrationDto
    {
        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; } = "";

        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; } = "";

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Phone number is required.")]
        public string PhoneNumber { get; set; } = "";

        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; } = "";

        [Required(ErrorMessage = "City is required.")]
        public string City { get; set; } = "";

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; } = "";
    }
}