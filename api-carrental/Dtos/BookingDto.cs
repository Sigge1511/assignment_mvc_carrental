using System.ComponentModel.DataAnnotations;

namespace api_carrental.Dtos
{
    public class BookingDto
    {
        public int Id { get; set; }

        // ********* Fordon *********

        [Required(ErrorMessage = "Vehicle ID is required.")]
        public int VehicleId { get; set; }

        public VehicleDto? Vehicle { get; set; }


        // ********* Identity-baserad användare *********
        [Required(ErrorMessage = "Customer ID is required.")]
        public string ApplicationUserId { get; set; } = "";
        //****
        [Required(ErrorMessage = "Customer information is required.")]
        public required ApplicationUserDto ApplicationUser { get; set; }



        // ********* Datum *********

        [Required(ErrorMessage = "Start date is required.")]
        public DateOnly StartDate { get; set; }

        [Required(ErrorMessage = "End date is required.")]
        public DateOnly EndDate { get; set; }


        // ********* Pris *********

        public double TotalPrice { get; set; } = 0.0;


    }
}
