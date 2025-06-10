using assignment_mvc_carrental.Classes;

namespace assignment_mvc_carrental.Repos
{
    public interface IVehicle
    {
        Task<List<Vehicle>> GetAllVehiclesAsync();
        Task<Vehicle> GetVehicleByIDAsync(int vehicleId);
        Task<Vehicle> UpdateVehicleAsync(Vehicle vehicle);
        Task DeleteVehicleAsync(Vehicle vehicle);
        Task<Vehicle> AddVehicleAsync(Vehicle vehicle);
    }
}
