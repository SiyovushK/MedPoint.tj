using AutoMapper;
using Domain.DTOs.DoctorDTOs;
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
    }
}