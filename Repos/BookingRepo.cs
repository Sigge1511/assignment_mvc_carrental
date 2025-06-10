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
        public async Task<BookingViewModel> GetBookingByIDAsync(int bookingId)
        {
            return await _context.BookingSet
                    .Where(b => b.Id == bookingId)
                    .Select(b => new BookingViewModel
                    {
                        Id = b.Id,
                        CustomerId = b.CustomerId,
                        CustomerName = _context.CustomerSet
                            .Where(c => c.Id == b.CustomerId)
                            .Select(c => c.FirstName + " " + c.LastName)
                            .FirstOrDefault(),
                        VehicleId = b.VehicleId,
                        VehicleName = _context.VehicleSet
                            .Where(v => v.Id == b.VehicleId)
                            .Select(v => v.Title)
                            .FirstOrDefault(),
                        StartDate = b.StartDate,
                        EndDate = b.EndDate,
                        TotalPrice = b.TotalPrice
                    })
                    .FirstOrDefaultAsync();
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
