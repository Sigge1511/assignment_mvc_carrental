using System.ComponentModel.DataAnnotations;

namespace assignment_mvc_carrental.Models
{
    public class BookingViewModel
    {
        public int Id { get; set; }
        public string VehicleTitle { get; set; } = "";


        [Required(ErrorMessage = "Vehicle ID is required.")]
        public int VehicleId { get; set; }


        [Required(ErrorMessage = "Customer ID is required.")]
        public int CustomerId { get; set; }


        [Required(ErrorMessage = "Firstname is required.")]
        public string CustomerFirstName { get; set; } = "";



        [Required(ErrorMessage = "Lastname is required.")]
        public string CustomerLastName { get; set; } = ""; 


        


        [Required(ErrorMessage = "Start date is required.")]
        public DateOnly StartDate { get; set; }
        [Required(ErrorMessage = "End date is required.")]
        public DateOnly EndDate { get; set; }
        public double TotalPrice { get; set; } = 0.0;
    }
}
