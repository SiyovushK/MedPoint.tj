using AutoMapper;
using Domain.DTOs.DoctorDTOs;
using Domain.DTOs.OrderDTOs;
using Domain.DTOs.ReviewDTOs;
using Domain.DTOs.ScheduleDTOs;
using Domain.DTOs.UserDTOs;
using Domain.Entities;

namespace Infrastructure.AutoMapper;

public class InfrastructureProfile : Profile
{
    public InfrastructureProfile()
    {
        CreateMap<CreateUserDTO, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.Orders, opt => opt.Ignore())
            .ForMember(dest => dest.Reviews, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.IsEmailVerified, opt => opt.Ignore())
            .ForMember(dest => dest.ResetToken, opt => opt.Ignore())
            .ForMember(dest => dest.ResetTokenExpiry, opt => opt.Ignore());
        CreateMap<UpdateUserDTO, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.Role, opt => opt.Ignore())
            .ForMember(dest => dest.Orders, opt => opt.Ignore())
            .ForMember(dest => dest.Reviews, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.IsEmailVerified, opt => opt.Ignore())
            .ForMember(dest => dest.ResetToken, opt => opt.Ignore())
            .ForMember(dest => dest.ResetTokenExpiry, opt => opt.Ignore())
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<User, GetUserDTO>()
            .ForMember(dest => dest.ProfileImageUrl, opt => opt.Ignore());

        CreateMap<CreateDoctorDTO, Doctor>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.Orders, opt => opt.Ignore())
            .ForMember(dest => dest.Reviews, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.ResetToken, opt => opt.Ignore())
            .ForMember(dest => dest.ResetTokenExpiry, opt => opt.Ignore());
        CreateMap<UpdateDoctorDTO, Doctor>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.Orders, opt => opt.Ignore())
            .ForMember(dest => dest.Reviews, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.ResetToken, opt => opt.Ignore())
            .ForMember(dest => dest.ResetTokenExpiry, opt => opt.Ignore())
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<Doctor, GetDoctorDTO>()
            .ForMember(dest => dest.ProfileImageUrl, opt => opt.Ignore());

        CreateMap<CreateOrderDTO, Order>();
        CreateMap<Order, GetOrderDTO>()
            .ForMember(dest => dest.DoctorName,
                opt => opt.MapFrom(src => $"{src.Doctor.FirstName} {src.Doctor.LastName}"))
            .ForMember(dest => dest.UserName,
                opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
            .ForMember(dest => dest.CreatedAt,
                opt => opt.MapFrom(src => src.CreatedAt.AddHours(5)));

        CreateMap<CreateReviewDTO, Review>();
        CreateMap<UpdateReviewDTO, Review>()
            .ForMember(dest => dest.Doctor, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore());
        CreateMap<Review, GetReviewDTO>()
            .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor != null
                ? $"{src.Doctor.FirstName} {src.Doctor.LastName}" : null))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
            .ForMember(dest => dest.CreatedAt,
                opt => opt.MapFrom(src => src.CreatedAt.AddHours(5)))
            .ForMember(dest => dest.UpdatedAt,
                opt => opt.MapFrom(src => src.UpdatedAt.AddHours(5)));

        CreateMap<CreateDoctorScheduleDTO, DoctorSchedule>();
        CreateMap<UpdateDoctorScheduleDTO, DoctorSchedule>();
        CreateMap<DoctorSchedule, GetDoctorScheduleDTO>()
            .ForMember(dest => dest.DoctorName,
                opt => opt.MapFrom(src => $"{src.Doctor.FirstName} {src.Doctor.LastName}"));
    }
}