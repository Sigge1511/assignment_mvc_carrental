using assignment_mvc_carrental.Classes;
using assignment_mvc_carrental.Models;

namespace assignment_mvc_carrental.Repos
{
    public interface IBooking
    {
        Task<IEnumerable<Booking>> GetAllBookingsAsync();
        Task<Booking?> GetBookingByIdAsync(int id);
        Task AddBookingAsync(Booking booking);
        Task UpdateBookingAsync(Booking booking);
        Task DeleteBookingAsync(int id);
    }
}
