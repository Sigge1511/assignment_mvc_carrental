using AutoMapper;
using api_carrental.Dtos;
using api_carrental.Models;

namespace api_carrental.Data
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Vehicle mappings
            CreateMap<Vehicle, VehicleDto>()
                .PreserveReferences(); // Handle circular references
            CreateMap<VehicleDto, Vehicle>()
                .PreserveReferences(); // Handle circular references

            // Booking mappings with nested objects
            CreateMap<Booking, BookingDto>()
                .ForMember(dest => dest.Vehicle, opt => opt.MapFrom(src => src.Vehicle))
                .ForMember(dest => dest.ApplicationUser, opt => opt.MapFrom(src => src.ApplicationUser))
                .PreserveReferences(); // Handle circular references

            CreateMap<BookingDto, Booking>()
                .ForMember(dest => dest.Vehicle, opt => opt.MapFrom(src => src.Vehicle))
                .ForMember(dest => dest.ApplicationUser, opt => opt.MapFrom(src => src.ApplicationUser))
                .PreserveReferences(); // Handle circular references

            // ApplicationUser mappings
            CreateMap<ApplicationUser, ApplicationUserDto>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .PreserveReferences(); // Handle circular references

            CreateMap<ApplicationUserDto, ApplicationUser>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .PreserveReferences(); // Handle circular references

            // User registration mapping
            CreateMap<UserRegistrationDto, ApplicationUserDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName));

            // Login user mapping
            CreateMap<LoginUserDto, ApplicationUserDto>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
        }
    }
}
