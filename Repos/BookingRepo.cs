using assignment_mvc_carrental.Classes;
using assignment_mvc_carrental.Data;
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

        //*************************************************************************************************

        public async Task<List<Booking>> GetAllBookingsAsync()
        {
            return await _context.BookingSet.ToListAsync();
        }
        //*************************************************************************************************
        public async Task<Booking> AddBookingAsync(Booking booking)
        {
            await _context.BookingSet.AddAsync(booking);
            await _context.SaveChangesAsync();
            return booking;
        }
        //*************************************************************************************************
        public async Task<Booking> GetBookingByIDAsync(int bookingId)
        {
            return await _context.BookingSet.FirstOrDefaultAsync(v => v.Id == bookingId);
        }
        //*************************************************************************************************
        
        public async Task<Booking> UpdateBookingAsync(Booking booking)
        {
            _context.BookingSet.Update(booking); // Update the vehicle in the context
            await _context.SaveChangesAsync(); // Save changes to the database
            return booking;
        }
        //*************************************************************************************************
        public async Task DeleteBookingAsync(Booking booking)
        {
            _context.BookingSet.Remove(booking);
            await _context.SaveChangesAsync();
        }        
    }
}
