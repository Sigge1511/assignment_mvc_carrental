using assignment_mvc_carrental.Classes;

namespace assignment_mvc_carrental.Repos
{
    public interface IVehicle
    {
        Task<List<Vehicle>> GetAllVehiclesAsync();
        Task<Vehicle> GetVehicleByIDAsync(int vehicleId);
        void UpdateVehicleAsync(Vehicle vehicle);
        void DeleteVehicleAsync(int vehicleId);
        void SaveVehicleAsync();
    }
}
