using assignment_mvc_carrental.Classes;

namespace assignment_mvc_carrental.Repos
{
    public interface IVehicle
    {
        Task<List<Vehicle>> GetAllVehiclesAsync();
        Vehicle GetVehicleByID(int vehicleId);
        void UpdateVehicle(Vehicle vehicle);
        void DeleteVehicle(int vehicleId);
        void SaveVehicle();
    }
}
