using api_carrental.Data;
using api_carrental.Dtos;
using api_carrental.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace api_carrental.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleDtController : ControllerBase
    {
        private readonly IVehicleRepo _vehicleRepo;

        public VehicleDtController(IVehicleRepo vehicleRepo)
        {
            _vehicleRepo = vehicleRepo;
        }
        //***************************************************************************************************************
        //HÄMTA ALLA FORDON
        // GET: api/<VehicleController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VehicleDto>>> GetIndexAsync()
        {
            var vehicles = await _vehicleRepo.GetAllVehiclesAsync(); 
            return Ok(vehicles);
        }

        //HÄMTA ENSKILT FORDON
        // GET api/<VehicleController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VehicleDto>> GetVehicleByIdAsync(int id)
        {            
            // Kollar om användaren är inloggad och om den är admin,
            // för att kunna visa admin-funktioner i vyn sen
            //ViewBag.IsAdmin = User.IsInRole("Admin");

            var vehicle = await _vehicleRepo.GetVehicleByIDAsync(id); 

            if (vehicle == null){return NotFound();}

            return Ok(vehicle);
        }

        //LÄGG TILL NYTT FORDON
        // POST api/<VehicleController>
        [Authorize(Roles = "Admin")] //Endast admin kan skapa fordon
        [HttpPost]
        public async Task<IActionResult> PostVehicleAsync([Bind("Id,Title,Year,PricePerDay,Description,ImageUrl1,ImageUrl2")] VehicleDto vehicle)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _vehicleRepo.AddVehicleAsync(vehicle);
                    return Ok(vehicle); //om det funkar ok
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message); //om det inte funkar badrequest
                }
            }
            return BadRequest("Invalid vehicle data.");
        }

        //UPPDATERA FORDON
        // PUT api/<VehicleController>/5
        [Authorize(Roles = "Admin")] // Endast admin kan uppdatera fordon
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVehicleAsync(int id, [FromBody] VehicleDto vehicle)
        {
            if (vehicle == null)
            {
                return BadRequest();
            }

            if (id != vehicle.Id)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                await _vehicleRepo.UpdateVehicleAsync(vehicle);
                return Ok(vehicle); //Rätt nu??
            }
            catch (DbUpdateConcurrencyException)
            {
                // Concurrency conflict
                return Conflict("An error occurred. Please try again.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //TA BORT FORDON
        // DELETE api/<VehicleController>/5
        [Authorize(Roles = "Admin")] //Endast admin kan radera fordon
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehicleAsync(int id)
        {
            try
            {
                var vehicle = await _vehicleRepo.GetVehicleByIDAsync(id); 
                if (vehicle == null)
                {
                    return BadRequest("Could not find the vehicle");
                }
                await _vehicleRepo.DeleteVehicleAsync(id); //anropar repo för att radera fordonet
                return Ok("Vehicle successfully deleted"); //skickar med en notis att fordonet är raderat
            }
            catch (Exception)
            {
                return BadRequest("An error occurred while deleting the vehicle.");
            }
        }
    }
}
