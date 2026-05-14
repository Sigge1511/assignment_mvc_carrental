using api_carrental.Data;
using api_carrental.Dtos;
using api_carrental.Repos;
using Microsoft.EntityFrameworkCore;

namespace api_carrental.Repos
{
    public class VehicleRepo : IVehicleRepo
    {
        private readonly ApplicationDbContext _context;

        public VehicleRepo(ApplicationDbContext context)
        {
            _context = context;
        }
//*************************************************************************************************
        public async Task<List<VehicleDto>> GetAllVehiclesAsync()
        {
            return await _context.VehicleSet.ToListAsync();
        }
        //*************************************************************************************************
        public async Task<VehicleDto> GetVehicleByIDAsync(int vehicleId)
        {
            return await _context.VehicleSet.FirstOrDefaultAsync(v => v.Id == vehicleId);
        }
        //*************************************************************************************************
        public async Task<VehicleDto> AddVehicleAsync(VehicleDto vehicle)
        {
            await _context.VehicleSet.AddAsync(vehicle);
            await _context.SaveChangesAsync();
            return vehicle;
        }
        //*************************************************************************************************
        public async Task<VehicleDto> UpdateVehicleAsync(VehicleDto vm) 
                                        //kolla vilken datatyp som ska användas här 
        {
            var vehicle = await _context.VehicleSet.FindAsync(vm.Id);

            if (vehicle == null)
            {
                return null;
            }
            //mappa/tilldela allt manuellt 
            vehicle.Title = vm.Title;
            vehicle.Year = vm.Year;
            vehicle.PricePerDay = vm.PricePerDay;
            vehicle.Description = vm.Description;
            vehicle.ImageUrl1 = vm.ImageUrl1;
            vehicle.ImageUrl2 = vm.ImageUrl2;

            await _context.SaveChangesAsync();
            return vehicle;
        }
        //*************************************************************************************************
        public async Task DeleteVehicleAsync(int id)
        {
            VehicleDto vehicle = await _context.VehicleSet
                .Where(v => v.Id == id)
                .FirstOrDefaultAsync();

            _context.VehicleSet.Remove(vehicle);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> IsVehicleAvailableAsync(int vehicleId, DateOnly startDate, DateOnly endDate)
        {
            // Kontrollerar om det finns NÅGON befintlig bokning för detta fordon
            // där bokningsperioden överlappar den nya perioden.

            // Överlappningslogik:
            // En ny period överlappar en befintlig om:
            // Den nya startdagen är INNAN den befintliga slutdagen OCH
            // Den nya slutdagen är EFTER den befintliga startdagen

            var overlappingBookings = await _context.BookingSet
                .Where(b => b.VehicleId == vehicleId)
                // Använder DateOnly jämförelser
                .Where(b => startDate <= b.EndDate && endDate >= b.StartDate)
                .AnyAsync(); // Använd AnyAsync för att få true/false snabbt

            // Returnerar true om INGA överlappande bokningar hittades.
            return !overlappingBookings;
        }
        //*************************************************************************************************
        public async Task<bool> IsVehicleAvailableForUpdateAsync(int bookingIdToExclude, int vehicleId, DateOnly startDate, DateOnly endDate)
        {
            // Söker efter överlappande bokningar, men ignorerar den bokning vi uppdaterar
            var overlappingBookingsFound = await _context.BookingSet 
                .Where(b => b.VehicleId == vehicleId) // Måste vara samma fordon
                .Where(b => b.Id != bookingIdToExclude) // Ignorera den aktuella bokningen
                .Where(b => startDate <= b.EndDate && endDate >= b.StartDate) // Kolla dagarna
                .AnyAsync(); //returnerar true om någon hittades

            // Returnerar true om INGA överlappande bokningar hittades
            // dvs om overlappingBookingsFound är false
            return !overlappingBookingsFound;
        }

    }
}
