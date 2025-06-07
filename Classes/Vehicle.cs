using System.ComponentModel.DataAnnotations;

namespace assignment_mvc_carrental.Classes
{
    public class Vehicle
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; } = "";


        public int Year { get; set; }

        public string Description { get; set; } = "";


        [Required(ErrorMessage = "Price is required.")]
        public double PricePerDay { get; set; }


        public bool IsAvailable { get; set; } = true;

        
        [Required(ErrorMessage = "Image URL is required.")]
        public string ImageUrl1 { get; set; } = "";


        [Required(ErrorMessage = "Image URL is required.")]
        public string ImageUrl2 { get; set; } = "";





        public Vehicle()
        {
           
        }
    }
}
