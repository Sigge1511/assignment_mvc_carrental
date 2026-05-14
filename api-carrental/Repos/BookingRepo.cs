using Microsoft.EntityFrameworkCore;
using api_carrental.Data;
using api_carrental.Dtos;

namespace api_carrental.Repos
{
    public class BookingRepo : IBookingRepo
    {
        private readonly ApplicationDbContext _context;

        public BookingRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BookingDto>> GetAllBookingsAsync()
        {
            return await _context.BookingSet.Include(b => b.Vehicle).Include(b => b.ApplicationUser).ToListAsync();
        }

        public async Task<BookingDto?> GetBookingByIdAsync(int id)
        {
            return await _context.BookingSet
                .Include(b => b.Vehicle)
                .Include(b => b.ApplicationUser) // hämta med info om bil och user med bokningen
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task AddBookingAsync(BookingDto booking)
        {
            _context.BookingSet.Add(booking);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBookingAsync(BookingDto booking)
        {
            _context.BookingSet.Update(booking);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBookingAsync(int id)
        {
            var booking = await _context.BookingSet.FindAsync(id);
            if (booking != null)
            {
                _context.BookingSet.Remove(booking);
                await _context.SaveChangesAsync();
            }
        }
    }

}
