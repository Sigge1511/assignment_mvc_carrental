using assignment_mvc_carrental.Models;
using assignment_mvc_carrental.ViewModels;
using AutoMapper;

namespace assignment_mvc_carrental.Data
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<VehicleViewModel, Vehicle>().ReverseMap();
            CreateMap<Vehicle, VehicleViewModel>().ReverseMap();

            CreateMap<Booking, BookingViewModel>()
                .ForMember(dest => dest.Vehicle, opt => opt.MapFrom(src => src.Vehicle))
                .ForMember(dest => dest.ApplicationUser, opt => opt.MapFrom(src => src.ApplicationUser));
            CreateMap<BookingViewModel, Booking>()
                .ForMember(dest => dest.Vehicle, opt => opt.MapFrom(src => src.Vehicle))
                .ForMember(dest => dest.ApplicationUser, opt => opt.MapFrom(src => src.ApplicationUser));

            CreateMap<AdminViewModel, ApplicationUser>().ReverseMap();
            CreateMap<ApplicationUser, AdminViewModel>().ReverseMap();

            CreateMap<UserInputViewModel, CustomerViewModel>().ReverseMap();
            CreateMap<CustomerViewModel, UserInputViewModel>().ReverseMap();
            CreateMap<ApplicationUser, CustomerViewModel>();


        }
    }
}
