using api_carrental.Dtos;

namespace api_carrental.Repos
{
    public interface IBookingRepo
    {
        Task<IEnumerable<BookingDto>> GetAllBookingsAsync();
        Task<BookingDto?> GetBookingByIdAsync(int id);
        Task AddBookingAsync(BookingDto booking);
        Task UpdateBookingAsync(BookingDto booking);
        Task DeleteBookingAsync(int id);
    }
}
