using assignment_mvc_carrental.Classes;
using assignment_mvc_carrental.Data;
using assignment_mvc_carrental.Models;
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
        public async Task<Vehicle> AddVehicleAsync(Vehicle vehicle)
        {
            await _context.VehicleSet.AddAsync(vehicle);
            await _context.SaveChangesAsync();
            return vehicle;
        }



        //*************************************************************************************************
        public async Task<Vehicle> UpdateVehicleAsync(VehicleViewModel vm)
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
            Vehicle vehicle = await _context.VehicleSet
                .Where(v => v.Id == id)
                .FirstOrDefaultAsync();

            _context.VehicleSet.Remove(vehicle);
            await _context.SaveChangesAsync();
        }

    }
}
