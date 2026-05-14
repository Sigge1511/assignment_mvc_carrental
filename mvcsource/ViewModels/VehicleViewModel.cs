using System.ComponentModel.DataAnnotations;

namespace assignment_mvc_carrental.ViewModels
{
    public class VehicleViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; } = "";


        public int Year { get; set; }


        [Required(ErrorMessage = "Price is required.")]
        public double PricePerDay { get; set; }


        public string Description { get; set; } = "";


        [Required(ErrorMessage = "Image URL is required.")]
        public string ImageUrl1 { get; set; } = "";


        [Required(ErrorMessage = "Image URL is required.")]
        public string ImageUrl2 { get; set; } = "";

        public ICollection<BookingViewModel>? Bookings { get; set; } // lista över bokningar som är kopplade till fordonet
    }
}
