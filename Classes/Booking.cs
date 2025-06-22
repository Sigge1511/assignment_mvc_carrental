﻿using System.ComponentModel.DataAnnotations;

namespace assignment_mvc_carrental.Classes
{
    public class Booking
    {
        public int Id { get; set; }

        // ********* Fordon *********
        public string VehicleTitle { get; set; } = "";

        [Required(ErrorMessage = "Vehicle ID is required.")]
        public int VehicleId { get; set; }

        public Vehicle? Vehicle { get; set; }


        // ********* Identity-baserad användare *********

        [Required(ErrorMessage = "Customer ID is required.")]
        public string ApplicationUserId { get; set; } = "";

        public ApplicationUser ApplicationUser { get; set; }


        // ********* Datum *********

        [Required(ErrorMessage = "Start date is required.")]
        public DateOnly StartDate { get; set; }

        [Required(ErrorMessage = "End date is required.")]
        public DateOnly EndDate { get; set; }


        // ********* Pris *********

        public double TotalPrice { get; set; } = 0.0;

     
    }
}
