using System.ComponentModel.DataAnnotations;

namespace assignment_mvc_carrental.Models
{
    public class BookingViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Vehicle ID is required.")]

        public string VehicleName { get; set; } = ""; // Changed to string to hold vehicle title

        public int VehicleId { get; set; }
        [Required(ErrorMessage = "Customer ID is required.")]

        public string CustomerFirstName { get; set; } = ""; 
        public string CustomerLastName { get; set; } = ""; 


        public int CustomerId { get; set; }
        [Required(ErrorMessage = "Start date is required.")]
        public DateOnly StartDate { get; set; }
        [Required(ErrorMessage = "End date is required.")]
        public DateOnly EndDate { get; set; }
        public double TotalPrice { get; set; } = 0.0;
    }
}
