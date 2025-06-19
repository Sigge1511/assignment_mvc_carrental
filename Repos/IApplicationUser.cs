using assignment_mvc_carrental.Classes;
using assignment_mvc_carrental.Models;
using Microsoft.AspNetCore.Identity;

namespace assignment_mvc_carrental.Repos
{
    public interface IApplicationUser
    {
        Task<List<ApplicationUserRepo>> GetAllUsersAsync();
        Task<ApplicationUserRepo> GetUserByIDAsync(int userId);
        Task<ApplicationUserRepo> UpdateCustomerAsync(ApplicationUserRepo appuser);
        Task DeleteUserAsync(ApplicationUserRepo appuser);
        Task<IdentityResult> AddCustomerAsync(CustomerViewModel customerVM);
    }
}

//public interface IVehicle
//{
//    Task<List<Vehicle>> GetAllVehiclesAsync();
//    Task<Vehicle> GetVehicleByIDAsync(int vehicleId);
//    Task<Vehicle> UpdateVehicleAsync(VehicleViewModel vm);
//    Task DeleteVehicleAsync(int id);
//    Task<Vehicle> AddVehicleAsync(Vehicle vehicle);
//}
