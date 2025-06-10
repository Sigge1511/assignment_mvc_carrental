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
        }
    }
}