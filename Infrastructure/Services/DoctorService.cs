using System.Net;
using System.Security.Claims;
using AutoMapper;
using Domain.DTOs.DoctorDTOs;
using Domain.Entities;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services;

public class DoctorService(
        IBaseRepository<Doctor, int> repository,
        DoctorRepository doctorRepository,
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

    public async Task<Response<string>> UploadOrUpdateProfileImageAsync(int doctorId, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return new Response<string>(HttpStatusCode.BadRequest, "File is empty or missing.");

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
            return new Response<string>(HttpStatusCode.BadRequest, "Only image files (.jpg, .jpeg, .png, .gif) are allowed.");

        if (file.Length > 10 * 1024 * 1024)
            return new Response<string>(HttpStatusCode.BadRequest, "File size cannot exceed 10MB.");

        var doctor = await doctorRepository.GetByIdAsync(doctorId);
        if (doctor == null)
            return new Response<string>(HttpStatusCode.NotFound, $"Doctor with ID {doctorId} not found.");

        // Deleting old image
        if (!string.IsNullOrEmpty(doctor.ProfileImagePath))
        {
            var oldFilePath = Path.Combine("wwwroot", doctor.ProfileImagePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
            if (File.Exists(oldFilePath))
                File.Delete(oldFilePath);
        }

        var folderPath = Path.Combine("wwwroot", "profile-images", "doctors");
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(folderPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        doctor.ProfileImagePath = $"/profile-images/doctors/{fileName}";
        await doctorRepository.UpdateAsync(doctor);

        return new Response<string>("Profile image uploaded successfully.");
    }

    public async Task<Response<string>> DeleteProfileImageAsync(int doctorId)
    {
        var doctor = await doctorRepository.GetByIdAsync(doctorId);
        if (doctor == null)
            return new Response<string>(HttpStatusCode.NotFound, "doctor not found.");

        if (string.IsNullOrEmpty(doctor.ProfileImagePath))
            return new Response<string>(HttpStatusCode.BadRequest, "No profile image to delete.");

        var filePath = Path.Combine("wwwroot", "images", "doctors", doctor.ProfileImagePath);
        if (File.Exists(filePath))
            File.Delete(filePath);

        doctor.ProfileImagePath = null;
        await doctorRepository.UpdateAsync(doctor);

        return new Response<string>("Profile image deleted successfully.");
    }

}