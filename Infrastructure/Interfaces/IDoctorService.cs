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
    Task<Response<GetDoctorDTO>> GetByIdAsync(int doctorId);
    Task<Response<GetDoctorDTO>> GetCurrentDoctorAsync(ClaimsPrincipal doctorClaims);
    Task<PagedResponse<List<GetDoctorDTO>>> GetAllAsync(DoctorFilter filter);
    Task<Response<string>> UploadOrUpdateProfileImageAsync(int doctorId, IFormFile image);
    Task<Response<string>> DeleteProfileImageAsync(int doctorId);
}