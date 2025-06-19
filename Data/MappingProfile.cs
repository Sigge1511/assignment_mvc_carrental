using assignment_mvc_carrental.Models;
using assignment_mvc_carrental.Classes;
using AutoMapper;


namespace assignment_mvc_carrental.Data
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<VehicleViewModel, Vehicle>().ReverseMap();
            CreateMap<Vehicle, VehicleViewModel>().ReverseMap();
            CreateMap<BookingViewModel, Booking>().ReverseMap();
            CreateMap<Booking, BookingViewModel>().ReverseMap();
            CreateMap<AdminViewModel, ApplicationUser>().ReverseMap();
            CreateMap<ApplicationUser, AdminViewModel>().ReverseMap();
            CreateMap<UserInputViewModel, CustomerViewModel>().ReverseMap();
            CreateMap<CustomerViewModel, UserInputViewModel>().ReverseMap();
            //.ForMember(dest => dest.Id, opt => opt.Ignore()) // Ignorera Id eftersom det inte ska mappas från CustomerViewModel
            //.ForMember(dest => dest.Email, opt => opt.Ignore()); // Ignorera Email eftersom det inte ska mappas från CustomerViewModel

        }
    }
}