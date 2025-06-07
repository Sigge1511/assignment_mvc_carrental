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


        public async Task<List<Vehicle>> GetAllVehiclesAsync()
        {
            return await _context.VehicleSet.ToListAsync();
        }

        public async Task<Vehicle> GetVehicleByIDAsync(int vehicleId)
        {            
            return await _context.VehicleSet.FirstOrDefaultAsync(v => v.Id == vehicleId);
        }

        public void SaveVehicleAsync()
        {
            throw new NotImplementedException();
        }

        public void UpdateVehicleAsync(Vehicle vehicle)
        {
            throw new NotImplementedException();
        }

        public void DeleteVehicleAsync(int vehicleId)
        {
            throw new NotImplementedException();
        }
    }
}
