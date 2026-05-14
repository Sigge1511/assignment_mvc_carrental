using api_carrental.Dtos;

namespace api_carrental.Repos
{
    public interface IVehicleRepo
    {
        Task<List<VehicleDto>> GetAllVehiclesAsync();
        Task<VehicleDto> GetVehicleByIDAsync(int vehicleId);
        Task<VehicleDto> UpdateVehicleAsync(VehicleDto vm);
        Task DeleteVehicleAsync(int id);
        Task<VehicleDto> AddVehicleAsync(VehicleDto vehicle);
        Task<bool> IsVehicleAvailableAsync(int vehicleId, DateOnly startDate, DateOnly endDate);
        Task<bool> IsVehicleAvailableForUpdateAsync(int bookingIdToExclude, int vehicleId, DateOnly startDate, DateOnly endDate);
    }
}
