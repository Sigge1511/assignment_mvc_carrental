using assignment_mvc_carrental.Classes;
using assignment_mvc_carrental.Models;

namespace assignment_mvc_carrental.Repos
{
    public interface IBooking
    {
        Task<List<Booking>> GetAllBookingsAsync();
        Task<BookingViewModel> GetBookingByIDAsync(int bookingId);
        Task<Booking> UpdateBookingAsync(Booking booking);
        Task DeleteBookingAsync(Booking booking);
        Task<Booking> AddBookingAsync(Booking booking);
    }
}
