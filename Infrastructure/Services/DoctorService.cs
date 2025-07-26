using System.Net;
using System.Security.Claims;
using System.Text.RegularExpressions;
using AutoMapper;
using Domain.DTOs.DoctorDTOs;
using Domain.DTOs.EmailDTOs;
using Domain.Entities;
using Domain.Enums;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Infrastructure.Services.HelperServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace Infrastructure.Services;

public class DoctorService(
        IBaseRepository<Doctor, int> repository,
        DoctorRepository doctorRepository,
        DoctorScheduleRepository doctorScheduleRepository,
        IMapper mapper,
        IPasswordHasher<Doctor> passwordHasher,
        IEmailService emailService,
        IHttpContextAccessor httpContextAccessor) : IDoctorService
{
    EmailVerification emailVerification = new();

    public async Task<Response<GetDoctorDTO>> CreateAsync(CreateDoctorDTO createDoctor)
    {
        createDoctor.FirstName = createDoctor.FirstName.Trim();
        createDoctor.LastName = createDoctor.LastName.Trim();
        createDoctor.Email = createDoctor.Email.Trim();
        createDoctor.Phone = createDoctor.Phone.Trim();
        createDoctor.Description = createDoctor.Description.Trim();

        // FirstName
        if (string.IsNullOrWhiteSpace(createDoctor.FirstName))
            return new Response<GetDoctorDTO>(HttpStatusCode.BadRequest, "First name can't be empty.");

        if (!createDoctor.FirstName.All(char.IsLetterOrDigit))
            return new Response<GetDoctorDTO>(HttpStatusCode.BadRequest, "First name must not contain spaces or special characters.");

        // LastName
        if (string.IsNullOrWhiteSpace(createDoctor.LastName))
            return new Response<GetDoctorDTO>(HttpStatusCode.BadRequest, "Last name can't be empty.");

        if (!createDoctor.LastName.All(char.IsLetterOrDigit))
            return new Response<GetDoctorDTO>(HttpStatusCode.BadRequest, "Last name must not contain spaces or special characters.");

        // Phone
        if (string.IsNullOrWhiteSpace(createDoctor.Phone))
            return new Response<GetDoctorDTO>(HttpStatusCode.BadRequest, "Phone can't be empty.");

        if (!createDoctor.Phone.All(char.IsDigit))
            return new Response<GetDoctorDTO>(HttpStatusCode.BadRequest, "Phone must contain only digits.");

        if (createDoctor.Phone.Length < 9)
            return new Response<GetDoctorDTO>(HttpStatusCode.BadRequest, "Phone must contain at least 9 digits.");

        var phoneExists = await doctorRepository.GetByPhoneAsync(createDoctor.Phone);
        if (phoneExists != null)
            return new Response<GetDoctorDTO>(HttpStatusCode.BadRequest, "Doctor with this phone already exists.");

        // Email
        if (string.IsNullOrWhiteSpace(createDoctor.Email))
            return new Response<GetDoctorDTO>(HttpStatusCode.BadRequest, "Email can't be empty.");

        try
        {
            var addr = new System.Net.Mail.MailAddress(createDoctor.Email);
            if (addr.Address != createDoctor.Email)
                return new Response<GetDoctorDTO>(HttpStatusCode.BadRequest, "Email format is invalid.");
        }
        catch
        {
            return new Response<GetDoctorDTO>(HttpStatusCode.BadRequest, "Email format is invalid.");
        }

        if (!emailVerification.EmailVerificationMethod(createDoctor.Email))
            return new Response<GetDoctorDTO>(HttpStatusCode.BadRequest, "Email domen is invalid.");

        var emailExists = await doctorRepository.GetByEmailAsync(createDoctor.Email);
        if (emailExists != null)
            return new Response<GetDoctorDTO>(HttpStatusCode.BadRequest, "Doctor with this email already exists.");

        // Password
        if (string.IsNullOrWhiteSpace(createDoctor.Password))
            return new Response<GetDoctorDTO>(HttpStatusCode.BadRequest, "Password can't be empty.");

        if (createDoctor.Password.Length < 4)
            return new Response<GetDoctorDTO>(HttpStatusCode.BadRequest, "Password must contain at least 4 characters.");

        if (!Regex.IsMatch(createDoctor.Password, @"^[a-zA-Z0-9]+$"))
            return new Response<GetDoctorDTO>(HttpStatusCode.BadRequest, "Password must contain only letters and digits (no symbols or spaces).");

        // Specialization
        if (createDoctor.Specialization == null || createDoctor.Specialization.Length == 0)
            return new Response<GetDoctorDTO>(HttpStatusCode.BadRequest, "At least one specialization must be selected.");

        var doctor = mapper.Map<Doctor>(createDoctor);
        doctor.PasswordHash = passwordHasher.HashPassword(doctor, createDoctor.Password);
        doctor.CreatedAt = DateTime.UtcNow.AddMonths(-3);

        if (await doctorRepository.AddAsync(doctor) == 0)
            return new Response<GetDoctorDTO>(HttpStatusCode.InternalServerError, "Doctor registration error!");

        // DoctorSchedule
        foreach (DayOfWeek dow in Enum.GetValues(typeof(DayOfWeek)))
        {
            var schedule = new DoctorSchedule
            {
                DoctorId = doctor.Id,
                DayOfWeek = dow,
                IsDayOff = dow == DayOfWeek.Sunday
            };

            if (!schedule.IsDayOff)
            {
                schedule.WorkStart = new TimeOnly(8, 0);
                schedule.WorkEnd = new TimeOnly(18, 0);
                schedule.LunchStart = new TimeOnly(12, 0);
                schedule.LunchEnd = new TimeOnly(13, 0);
            }

            await doctorScheduleRepository.AddAsync(schedule);
        }

        var emailDto = new EmailDTO
        {
            To = doctor.Email,
            Subject = "Registration info",
            Body = $"Hello doctor {doctor.FirstName},\n\nYour account has been registegred successfully"
        };
        await emailService.SendEmailAsync(emailDto);

        var getDoctorDTO = mapper.Map<GetDoctorDTO>(doctor);

        return new Response<GetDoctorDTO>(getDoctorDTO);
    }

    public async Task<Response<GetDoctorDTO>> UpdateAsync(int id, UpdateDoctorDTO updateDoctor)
    {
        updateDoctor.FirstName = updateDoctor.FirstName.Trim();
        updateDoctor.LastName = updateDoctor.LastName.Trim();
        updateDoctor.Email = updateDoctor.Email.Trim();
        updateDoctor.Phone = updateDoctor.Phone.Trim();
        updateDoctor.Description = updateDoctor.Description.Trim();

        // Id
        var checkDoctor = await doctorRepository.GetByIdAsync(id);
        if (checkDoctor == null || checkDoctor.IsDeleted == true)
            return new Response<GetDoctorDTO>(HttpStatusCode.BadRequest, $"Doctor with id {id} is not found");

        // FirstName
        if (string.IsNullOrWhiteSpace(updateDoctor.FirstName))
            return new Response<GetDoctorDTO>(HttpStatusCode.BadRequest, "First name can't be empty.");

        if (!updateDoctor.FirstName.All(char.IsLetterOrDigit))
            return new Response<GetDoctorDTO>(HttpStatusCode.BadRequest, "First name must not contain spaces or special characters.");

        // LastName
        if (string.IsNullOrWhiteSpace(updateDoctor.LastName))
            return new Response<GetDoctorDTO>(HttpStatusCode.BadRequest, "Last name can't be empty.");

        if (!updateDoctor.LastName.All(char.IsLetterOrDigit))
            return new Response<GetDoctorDTO>(HttpStatusCode.BadRequest, "Last name must not contain spaces or special characters.");

        // Phone
        if (string.IsNullOrWhiteSpace(updateDoctor.Phone))
            return new Response<GetDoctorDTO>(HttpStatusCode.BadRequest, "Phone can't be empty.");

        if (!updateDoctor.Phone.All(char.IsDigit))
            return new Response<GetDoctorDTO>(HttpStatusCode.BadRequest, "Phone must contain only digits.");

        if (updateDoctor.Phone.Length < 9)
            return new Response<GetDoctorDTO>(HttpStatusCode.BadRequest, "Phone must contain at least 9 digits.");

        if (!string.Equals(updateDoctor.Phone, checkDoctor.Phone, StringComparison.OrdinalIgnoreCase))
        {
            var phoneCheck = await doctorRepository.GetByPhoneAsync(updateDoctor.Phone);
            if (phoneCheck != null)
                return new Response<GetDoctorDTO>(HttpStatusCode.BadRequest, "Doctor with this phone number is already registered");
        }

        // Email
        if (string.IsNullOrWhiteSpace(updateDoctor.Email))
            return new Response<GetDoctorDTO>(HttpStatusCode.BadRequest, "Email can't be empty.");

        try
        {
            var addr = new System.Net.Mail.MailAddress(updateDoctor.Email);
            if (addr.Address != updateDoctor.Email)
                return new Response<GetDoctorDTO>(HttpStatusCode.BadRequest, "Email format is invalid.");
        }
        catch
        {
            return new Response<GetDoctorDTO>(HttpStatusCode.BadRequest, "Email format is invalid.");
        }

        if (!emailVerification.EmailVerificationMethod(updateDoctor.Email))
            return new Response<GetDoctorDTO>(HttpStatusCode.BadRequest, "Email domen is invalid.");

        if (!string.Equals(updateDoctor.Email, checkDoctor.Email, StringComparison.OrdinalIgnoreCase))
        {
            var emailCheck = await doctorRepository.GetByEmailAsync(updateDoctor.Email);
            if (emailCheck != null)
                return new Response<GetDoctorDTO>(HttpStatusCode.BadRequest, "Doctor with this email is already registered");
        }

        // Specialization
        if (updateDoctor.Specialization == null || updateDoctor.Specialization.Length == 0)
            return new Response<GetDoctorDTO>(HttpStatusCode.BadRequest, "At least one specialization must be selected.");

        mapper.Map(updateDoctor, checkDoctor);

        if (await doctorRepository.UpdateAsync(checkDoctor) == 0)
            return new Response<GetDoctorDTO>(HttpStatusCode.BadRequest, "Doctor update error");

        var emailDto = new EmailDTO
        {
            To = checkDoctor.Email,
            Subject = "Account info",
            Body = $"Hello doctor {checkDoctor.FirstName},\n\nYour account has been updated successfully"
        };
        await emailService.SendEmailAsync(emailDto);

        var getDoctorDTO = mapper.Map<GetDoctorDTO>(checkDoctor);

        return new Response<GetDoctorDTO>(getDoctorDTO);
    }

    public async Task<Response<string>> DeleteAsync(int doctorId)
    {
        var doctor = await doctorRepository.GetByIdAsync(doctorId);
        if (doctor == null)
            return new Response<string>(HttpStatusCode.NotFound, $"Doctor with id {doctorId} is not found");

        if (await doctorRepository.DeleteAsync(doctor) == 0)
            return new Response<string>(HttpStatusCode.InternalServerError, "Doctor couldn't be deleted");

        var emailDto = new EmailDTO
        {
            To = doctor.Email,
            Subject = "Account info",
            Body = $"Hello {doctor.FirstName},\n\nYour account has been deleted successfully"
        };
        await emailService.SendEmailAsync(emailDto);

        return new Response<string>("Doctor deleted successfully");
    }

    public async Task<Response<GetDoctorDTO>> ActivateOrDisableAsync(ChangeDoctorActivityStatus doctorActivity)
    {
        var doctor = await doctorRepository.GetByIdAsync(doctorActivity.DoctorId);
        if (doctor == null || doctor.IsDeleted)
            return new Response<GetDoctorDTO>(HttpStatusCode.NotFound, "Doctor not found");

        doctor.IsActive = doctorActivity.IsActive;

        var updated = await doctorRepository.UpdateAsync(doctor);
        if (updated == 0)
        {
            var action = doctorActivity.IsActive ? "activate" : "deactivate";
            return new Response<GetDoctorDTO>(HttpStatusCode.InternalServerError, $"Failed to {action} the doctor.");
        }

        var getDoctorDTO = mapper.Map<GetDoctorDTO>(doctor);

        return new Response<GetDoctorDTO>(getDoctorDTO);
    }

    public async Task<Response<GetDoctorDTO>> GetByIdAsync(int doctorId)
    {
        var doctor = await doctorRepository.GetByIdAsync(doctorId);
        if (doctor == null)
            return new Response<GetDoctorDTO>(HttpStatusCode.NotFound, $"Doctor with id {doctorId} is not found");

        var getDoctorDto = mapper.Map<GetDoctorDTO>(doctor);

        var request = httpContextAccessor.HttpContext?.Request;
        if (!string.IsNullOrEmpty(doctor.ProfileImagePath) && request != null)
        {
            getDoctorDto.ProfileImageUrl = $"{request.Scheme}://{request.Host}{doctor.ProfileImagePath}";
        }

        return new Response<GetDoctorDTO>(getDoctorDto);
    }

    public async Task<Response<List<GetDoctorDTO>>> GetByNameAsync(string name)
    {
        var doctors = await doctorRepository.GetByDoctorNameAsync(name);
        if (doctors == null)
            return new Response<List<GetDoctorDTO>>(HttpStatusCode.NotFound, $"No matching doctors found");

        var getDoctorDto = mapper.Map<List<GetDoctorDTO>>(doctors);

        var request = httpContextAccessor.HttpContext?.Request;
        for (int i = 0; i < doctors.Count; i++)
        {
            if (!string.IsNullOrEmpty(doctors[i].ProfileImagePath) && request != null)
            {
                getDoctorDto[i].ProfileImageUrl = $"{request.Scheme}://{request.Host}{doctors[i].ProfileImagePath}";
            }
        }

        return new Response<List<GetDoctorDTO>>(getDoctorDto);
    }

    public async Task<Response<GetDoctorDTO>> GetCurrentDoctorAsync(ClaimsPrincipal doctorClaims)
    {
        var doctorIdClaim = doctorClaims.FindFirst(ClaimTypes.NameIdentifier);
        if (doctorIdClaim == null)
            return new Response<GetDoctorDTO>(HttpStatusCode.Unauthorized, "Doctor ID not found in token");

        if (!int.TryParse(doctorIdClaim.Value, out int doctorId))
            return new Response<GetDoctorDTO>(HttpStatusCode.BadRequest, "Invalid doctor ID in token");

        var doctor = await doctorRepository.GetByIdAsync(doctorId);
        if (doctor == null || doctor.IsDeleted)
            return new Response<GetDoctorDTO>(HttpStatusCode.NotFound, "Doctor not found");

        var getDoctorDto = mapper.Map<GetDoctorDTO>(doctor);

        var request = httpContextAccessor.HttpContext?.Request;
        if (!string.IsNullOrEmpty(doctor.ProfileImagePath) && request != null)
        {
            getDoctorDto.ProfileImageUrl = $"{request.Scheme}://{request.Host}{doctor.ProfileImagePath}";
        }

        return new Response<GetDoctorDTO>(getDoctorDto);
    }

    public async Task<Response<List<GetDoctorDTO>>> GetAllAsync(DoctorFilter filter)
    {
        var query = repository.GetAll();

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            var search = filter.Name.ToLower();
            query = query.Where(d =>
                d.FirstName.ToLower().Contains(search) ||
                d.LastName.ToLower().Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(filter.Email))
            query = query.Where(d => d.Email.ToLower().Contains(filter.Email.ToLower()));

        if (filter.Specialization != null && filter.Specialization.Length > 0)
            query = query.Where(d => d.Specialization.Any(s => filter.Specialization.Contains(s)));

        if (filter.IsDeleted.HasValue)
            query = query.Where(d => d.IsDeleted == filter.IsDeleted.Value);

        if (filter.IsActive.HasValue)
            query = query.Where(d => d.IsActive == filter.IsActive.Value);

        var doctors = await query.ToListAsync();

        var getDoctorsDto = mapper.Map<List<GetDoctorDTO>>(doctors);

        var request = httpContextAccessor.HttpContext?.Request;
        if (request != null)
        {
            var baseUrl = $"{request.Scheme}://{request.Host}";

            foreach (var dto in getDoctorsDto)
            {
                var user = doctors.FirstOrDefault(u => u.Id == dto.Id);
                if (user != null && !string.IsNullOrEmpty(user.ProfileImagePath))
                {
                    dto.ProfileImageUrl = $"{baseUrl}{user.ProfileImagePath}";
                }
            }
        }

        return new Response<List<GetDoctorDTO>>(getDoctorsDto);
    }

    public async Task<Response<string>> UploadOrUpdateProfileImageAsync(ClaimsPrincipal doctorClaims, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return new Response<string>(HttpStatusCode.BadRequest, "File is empty or missing.");

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
            return new Response<string>(HttpStatusCode.BadRequest, "Only image files (.jpg, .jpeg, .png, .gif) are allowed.");

        if (file.Length > 10 * 1024 * 1024)
            return new Response<string>(HttpStatusCode.BadRequest, "File size cannot exceed 10MB.");

        // Id from token
        var doctorIdClaim = doctorClaims.FindFirst(ClaimTypes.NameIdentifier);
        if (doctorIdClaim == null || !int.TryParse(doctorIdClaim.Value, out int doctorId))
            return new Response<string>(HttpStatusCode.Unauthorized, "Doctor ID not found or invalid in token.");

        var doctor = await doctorRepository.GetByIdAsync(doctorId);
        if (doctor == null || doctor.IsDeleted)
            return new Response<string>(HttpStatusCode.NotFound, $"Doctor with ID {doctorId} not found.");

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

    public async Task<Response<string>> DeleteProfileImageAsync(ClaimsPrincipal doctorClaims)
    {
        var doctorIdClaim = doctorClaims.FindFirst(ClaimTypes.NameIdentifier);
        if (doctorIdClaim == null || !int.TryParse(doctorIdClaim.Value, out int doctorId))
            return new Response<string>(HttpStatusCode.Unauthorized, "Doctor ID not found or invalid in token.");

        var doctor = await doctorRepository.GetByIdAsync(doctorId);
        if (doctor == null || doctor.IsDeleted)
            return new Response<string>(HttpStatusCode.NotFound, "Doctor not found.");

        if (string.IsNullOrEmpty(doctor.ProfileImagePath))
            return new Response<string>(HttpStatusCode.BadRequest, "No profile image to delete.");

        var filePath = Path.Combine("wwwroot", doctor.ProfileImagePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
        if (File.Exists(filePath))
            File.Delete(filePath);

        doctor.ProfileImagePath = null;
        await doctorRepository.UpdateAsync(doctor);

        return new Response<string>("Profile image deleted successfully.");
    }

    public async Task<Response<List<SpecializationDTO>>> GetSpecializationsAsync()
    {
        var specializations = Enum.GetValues(typeof(DoctorSpecialization))
            .Cast<DoctorSpecialization>()
            .Select(e => new SpecializationDTO
            {
                Id = (int)e,
                Name = e.ToString()
            })
            .ToList();

        return new Response<List<SpecializationDTO>>(specializations);
    }

    public async Task<Response<DoctorStatisticsDTO>> GetDoctorStatisticsAsync(ClaimsPrincipal doctorClaims)
    {
        var doctorIdClaim = doctorClaims.FindFirst(ClaimTypes.NameIdentifier);
        if (doctorIdClaim == null || !int.TryParse(doctorIdClaim.Value, out int doctorId))
            return new Response<DoctorStatisticsDTO>(HttpStatusCode.Unauthorized, "Doctor ID not found or invalid in token.");
            
        var getPopularDoctors = await doctorRepository.GetDoctorStatistics(doctorId);
        return new Response<DoctorStatisticsDTO>(getPopularDoctors);
    }
}