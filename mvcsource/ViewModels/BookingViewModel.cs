using assignment_mvc_carrental.Models;
using System.ComponentModel.DataAnnotations;

namespace assignment_mvc_carrental.ViewModels
{
    public class BookingViewModel
    {
        public int Id { get; set; }

        // ********* Fordon *********

        [Required(ErrorMessage = "Vehicle ID is required.")]
        public int VehicleId { get; set; }

        public Vehicle? Vehicle { get; set; }


        // ********* Identity-baserad användare *********

        public string? ApplicationUserId { get; set; }

        public ApplicationUser? ApplicationUser { get; set; }


        // ********* Datum *********

        [Required(ErrorMessage = "Start date is required.")]
        public DateOnly StartDate { get; set; }

        [Required(ErrorMessage = "End date is required.")]
        public DateOnly EndDate { get; set; }


        // ********* Pris *********
        public double TotalPrice { get; set; } = 0.0;
    }
}

