using assignment_mvc_carrental.Classes;

namespace assignment_mvc_carrental.Repos
{
    public interface IBooking
    {
        Task<List<Booking>> GetAllBookingsAsync();
        Task<Booking> GetBookingByIDAsync(int bookingId);
        Task<Booking> UpdateBookingAsync(Booking booking);
        Task DeleteBookingAsync(Booking booking);
        Task<Booking> AddBookingAsync(Booking booking);
    }
}
