using assignment_mvc_carrental.Classes;
using assignment_mvc_carrental.Data;
using assignment_mvc_carrental.Models;
using Microsoft.EntityFrameworkCore;

namespace assignment_mvc_carrental.Repos
{
    public class BookingRepo : IBooking
    {
        private readonly ApplicationDbContext _context;

        public BookingRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Booking>> GetAllBookingsAsync()
        {
            return await _context.BookingSet.Include(b => b.Vehicle).Include(b => b.ApplicationUser).ToListAsync();
        }

        public async Task<Booking?> GetBookingByIdAsync(int id)
        {
            return await _context.BookingSet.FindAsync(id);
        }

        public async Task AddBookingAsync(Booking booking)
        {
            _context.BookingSet.Add(booking);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBookingAsync(Booking booking)
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
