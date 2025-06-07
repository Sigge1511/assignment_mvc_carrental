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

        public Vehicle GetVehicleByID(int vehicleId)
        {
            throw new NotImplementedException();
        }

        public void SaveVehicle()
        {
            throw new NotImplementedException();
        }

        public void UpdateVehicle(Vehicle vehicle)
        {
            throw new NotImplementedException();
        }

        public void DeleteVehicle(int vehicleId)
        {
            throw new NotImplementedException();
        }
    }
}
