using assignment_mvc_carrental.Classes;
using System.ComponentModel.DataAnnotations;

namespace assignment_mvc_carrental.Models
{
    public class AdminViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
