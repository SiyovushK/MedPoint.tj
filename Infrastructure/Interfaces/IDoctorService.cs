using System.Security.Claims;
using Domain.DTOs.DoctorDTOs;
using Domain.Filters;
using Domain.Responses;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Interfaces;

public interface IDoctorService
{
    Task<Response<GetDoctorDTO>> CreateAsync(CreateDoctorDTO createDoctor);
    Task<Response<GetDoctorDTO>> UpdateAsync(int id, UpdateDoctorDTO updateDoctor);
    Task<Response<string>> DeleteAsync(int doctorId);
    Task<Response<GetDoctorDTO>> ActivateOrDisableAsync(ChangeDoctorActivityStatus doctorActivity);
    Task<Response<GetDoctorDTO>> GetByIdAsync(int doctorId);
    Task<Response<GetDoctorDTO>> GetCurrentDoctorAsync(ClaimsPrincipal doctorClaims);
    Task<Response<List<GetDoctorDTO>>> GetAllAsync(DoctorFilter filter);
    Task<Response<string>> UploadOrUpdateProfileImageAsync(ClaimsPrincipal doctorClaims, IFormFile file);
    Task<Response<string>> DeleteProfileImageAsync(ClaimsPrincipal doctorClaims);
    Task<Response<List<SpecializationDTO>>> GetSpecializationsAsync();
}