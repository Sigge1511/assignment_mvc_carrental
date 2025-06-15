using assignment_mvc_carrental.Classes;
using assignment_mvc_carrental.Models;

namespace assignment_mvc_carrental.Repos
{
    public interface IVehicle
    {
        Task<List<Vehicle>> GetAllVehiclesAsync();
        Task<Vehicle> GetVehicleByIDAsync(int vehicleId);
        Task<Vehicle> UpdateVehicleAsync(VehicleViewModel vm);
        Task DeleteVehicleAsync(int id);
        Task<Vehicle> AddVehicleAsync(Vehicle vehicle);
    }
}
