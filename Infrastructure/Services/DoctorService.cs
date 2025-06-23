using System.Security.Claims;
using AutoMapper;
using Domain.DTOs.DoctorDTOs;
using Domain.Entities;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services;

public class DoctorService(
        IBaseRepository<Doctor, int> repository,
        DoctorRepository DoctorRepository,
        IMapper mapper,
        IPasswordHasher<Doctor> passwordHasher,
        IEmailService emailService) : IDoctorService
{
    public Task<Response<GetDoctorDTO>> CreateAsync(CreateDoctorDTO createDoctor)
    {
        throw new NotImplementedException();
    }

    public Task<PagedResponse<List<GetDoctorDTO>>> GetAllAsync(DoctorFilter filter)
    {
        throw new NotImplementedException();
    }

    public Task<Response<GetDoctorDTO>> GetByIdAsync(int doctorId)
    {
        throw new NotImplementedException();
    }

    public Task<Response<GetDoctorDTO>> GetCurrentDoctorAsync(ClaimsPrincipal doctorClaims)
    {
        throw new NotImplementedException();
    }

    public Task<Response<GetDoctorDTO>> UpdateAsync(int id, UpdateDoctorDTO updateDoctor)
    {
        throw new NotImplementedException();
    }
}