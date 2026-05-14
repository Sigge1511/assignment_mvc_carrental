using Microsoft.AspNetCore.Mvc;
using api_carrental.Repos;
using api_carrental.Dtos;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace api_carrental.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
        
    public class BookingDtController : ControllerBase
    {
        private readonly IBookingRepo _bookingRepo;
        private readonly IVehicleRepo _vehicleRepo;

        public BookingDtController(IBookingRepo bookingRepo, IVehicleRepo vehicleRepo)
        {
            _bookingRepo = bookingRepo;
            _vehicleRepo = vehicleRepo;
        }
//******************* HÄMTA ALLA BOKNINGAR ***********************
        // GET: api/<BookingController>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetIndexAsync()
        {
            var bookings = await _bookingRepo.GetAllBookingsAsync();
            return Ok(bookings);
        }
//******************* HÄMTA EN BOKNING VIA ID ***********************
        // GET api/<BookingController>/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<BookingDto>> Get(int id)
        {
            var booking = await _bookingRepo.GetBookingByIdAsync(id);
            if (booking is null) return NotFound();
            return Ok(booking);
        }
//****************************************************************************************
        // POST api/<BookingController>
        // DVS SKAPA NY BOKNING
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<BookingDto>> Post([FromBody] BookingDto booking)
        {
            var vehicle = await _vehicleRepo.GetVehicleByIDAsync(booking.VehicleId);
            if (vehicle == null)
            {
                // Returnerar 404 Not Found om fordonet inte existerar
                return NotFound($"Vehicle with ID {booking.VehicleId} not found.");
            }
            var days = (booking.EndDate.DayNumber - booking.StartDate.DayNumber) + 1;
            // Kolla om bilen är tillgänglig            
                bool isAvailable = await _vehicleRepo.IsVehicleAvailableAsync(
                booking.VehicleId,
                booking.StartDate,
                booking.EndDate
            );

            if (!isAvailable)
            {
                return BadRequest("The selected vehicle is already booked during the specified period.");
            }
            booking.TotalPrice = vehicle.PricePerDay * days;            
            await _bookingRepo.AddBookingAsync(booking);

            return CreatedAtAction(nameof(Get), new { id = booking.Id }, booking);
        }
//****************************************************************************************    
        // PUT api/<BookingController>/5
        // DVS UPPDATERA BOKNING
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] BookingDto bookingDto)
        {
            if (id != bookingDto.Id)
            {
                return BadRequest("Booking ID mismatch.");
            }

            var existingBooking = await _bookingRepo.GetBookingByIdAsync(id);
            if (existingBooking == null)
            {
                return NotFound($"Booking with ID {id} not found.");
            }

            // Kolla om bokningen verkligen behöver uppdateras
            // eller om datan är oförändrad!!
            bool isDataUnchanged =
                existingBooking.VehicleId == bookingDto.VehicleId &&
                existingBooking.StartDate == bookingDto.StartDate &&
                existingBooking.EndDate == bookingDto.EndDate &&
                existingBooking.ApplicationUserId == bookingDto.ApplicationUserId;
            if (isDataUnchanged)
            {
                // 304 Not Modified är den korrekta HTTP-statusen här.
                return StatusCode(304);
            }            

            var vehicle = await _vehicleRepo.GetVehicleByIDAsync(bookingDto.VehicleId);
            if (vehicle == null)
            {
                return NotFound($"Vehicle with ID {bookingDto.VehicleId} not found.");
            }

            bool isAvailable = await _vehicleRepo.IsVehicleAvailableForUpdateAsync(
                bookingDto.Id,
                bookingDto.VehicleId,
                bookingDto.StartDate,
                bookingDto.EndDate
            );
            if (!isAvailable)
            {
                return Conflict("The selected vehicle is not available during this updated period.");
            }
            var days = (bookingDto.EndDate.DayNumber - bookingDto.StartDate.DayNumber) + 1;
            var totalPrice = vehicle.PricePerDay * days;

            existingBooking.VehicleId = bookingDto.VehicleId;
            existingBooking.StartDate = bookingDto.StartDate;
            existingBooking.EndDate = bookingDto.EndDate;
            existingBooking.TotalPrice = totalPrice;
            //kundid ändras inte här

            try
            {
                await _bookingRepo.UpdateBookingAsync(existingBooking);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Failed to save changes to the database. " + ex.Message);
            }
            return Ok("Successful update.");
        }

        // DELETE api/<BookingController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _bookingRepo.DeleteBookingAsync(id);
            return NoContent();
        }
    }
}
