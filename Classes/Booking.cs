using System.ComponentModel.DataAnnotations;

namespace assignment_mvc_carrental.Classes
{
    public class Booking
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Vehicle ID is required.")]
        public int VehicleId { get; set; }
        [Required(ErrorMessage = "Customer ID is required.")]
        public int CustomerId { get; set; }
        [Required(ErrorMessage = "Start date is required.")]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "End date is required.")]
        public DateTime EndDate { get; set; }
        public double TotalPrice { get; set; } = 0.0;

        public Booking()
        {
            
        }
    }
}
