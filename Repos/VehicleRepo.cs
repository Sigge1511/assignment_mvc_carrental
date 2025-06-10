using assignment_mvc_carrental.Classes;
using assignment_mvc_carrental.Data;
using Microsoft.EntityFrameworkCore;

namespace assignment_mvc_carrental.Repos
{
    public class VehicleRepo : IVehicle
    {
        private readonly ApplicationDbContext _context;

        public VehicleRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        //*************************************************************************************************
        public async Task<List<Vehicle>> GetAllVehiclesAsync()
        {
            return await _context.VehicleSet.ToListAsync();
        }
        //*************************************************************************************************

        public async Task<Vehicle> GetVehicleByIDAsync(int vehicleId)
        {            
            return await _context.VehicleSet.FirstOrDefaultAsync(v => v.Id == vehicleId);
        }
        //*************************************************************************************************

        public async Task<Vehicle> UpdateVehicleAsync(Vehicle vehicle)
        {
            _context.VehicleSet.Update(vehicle); // Update the vehicle in the context
            await _context.SaveChangesAsync(); // Save changes to the database
            return vehicle;
        }
        //*************************************************************************************************

        public async Task<Vehicle> AddVehicleAsync(Vehicle vehicle)
        {
            await _context.VehicleSet.AddAsync(vehicle);
            await _context.SaveChangesAsync();
            return vehicle;
        }
        //*************************************************************************************************

        public async Task DeleteVehicleAsync(Vehicle vehicle)
        {
            _context.VehicleSet.Remove(vehicle);
            await _context.SaveChangesAsync();
        }
    }
}
