using assignment_mvc_carrental.Classes;
using System.ComponentModel.DataAnnotations;

namespace assignment_mvc_carrental.Models
{
    public class AdminViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; } = "";
        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; } = "";
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public UserRole UserRole { get; set; } = UserRole.AdminRole;
    }
}
