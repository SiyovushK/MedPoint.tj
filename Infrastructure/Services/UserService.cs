using System.Net;
using System.Security.Claims;
using System.Text.RegularExpressions;
using AutoMapper;
using Domain.Constants;
using Domain.DTOs.EmailDTOs;
using Domain.DTOs.UserDTOs;
using Domain.Entities;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Infrastructure.Services.HelperServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class UserService(
        IBaseRepository<User, int> repository,
        UserRepository userRepository,
        DoctorRepository doctorRepository,
        IMapper mapper,
        IPasswordHasher<User> passwordHasher,
        IPasswordHasher<Doctor> passwordHasherDoctor,
        IEmailService emailService,
        IHttpContextAccessor httpContextAccessor) : IUserService
{
    EmailVerification emailVerification = new();

    public async Task<Response<GetUserDTO>> CreateAsync(CreateUserDTO createUser)
    {
        createUser.FirstName = createUser.FirstName.Trim();
        createUser.LastName = createUser.LastName.Trim();
        createUser.Email = createUser.Email.Trim();
        createUser.Phone = createUser.Phone.Trim();

        //FirstName
        if (string.IsNullOrWhiteSpace(createUser.FirstName))
            return new Response<GetUserDTO>(HttpStatusCode.BadRequest, "First Name can't be empty");

        if (!createUser.FirstName.All(char.IsLetterOrDigit))
            return new Response<GetUserDTO>(HttpStatusCode.BadRequest, "First Name must not contain spaces or special characters!");

        //LastName
        if (string.IsNullOrWhiteSpace(createUser.LastName))
            return new Response<GetUserDTO>(HttpStatusCode.BadRequest, "Last Name can't be empty");

        if (!createUser.LastName.All(char.IsLetterOrDigit))
            return new Response<GetUserDTO>(HttpStatusCode.BadRequest, "Last Name must not contain spaces or special characters!");

        //Phone
        if (string.IsNullOrWhiteSpace(createUser.Phone))
            return new Response<GetUserDTO>(HttpStatusCode.BadRequest, "Phone can't be empty");

        if (!createUser.Phone.All(char.IsDigit))
            return new Response<GetUserDTO>(HttpStatusCode.BadRequest, "Phone must contain only numbers, no spaces or special characters allowed!");

        if (createUser.Phone.Length < 9)
            return new Response<GetUserDTO>(HttpStatusCode.BadRequest, "Phone size must contain at least 9 digits");

        var phoneCheck = await userRepository.GetByPhoneAsync(createUser.Phone.Trim());
        if (phoneCheck != null)
            return new Response<GetUserDTO>(HttpStatusCode.BadRequest, "User with this phone numbers alredy registered");

        //Email
        if (string.IsNullOrWhiteSpace(createUser.Email))
            return new Response<GetUserDTO>(HttpStatusCode.BadRequest, "Email can't be empty");

        try
        {
            var addr = new System.Net.Mail.MailAddress(createUser.Email);
            if (addr.Address != createUser.Email)
                return new Response<GetUserDTO>(HttpStatusCode.BadRequest, "Email format is invalid");
        }
        catch
        {
            return new Response<GetUserDTO>(HttpStatusCode.BadRequest, "Email format is invalid");
        }

        if (!emailVerification.EmailVerificationMethod(createUser.Email))
            return new Response<GetUserDTO>(HttpStatusCode.BadRequest, "Email domen is invalid");

        var emailCheck = await userRepository.GetByEmailAsync(createUser.Email);
        if (emailCheck != null)
            return new Response<GetUserDTO>(HttpStatusCode.BadRequest, "User with this email is alredy registered");

        //Password
        if (string.IsNullOrWhiteSpace(createUser.Password))
            return new Response<GetUserDTO>(HttpStatusCode.BadRequest, "Password can't be empty");

        if (createUser.Password.Length < 4)
            return new Response<GetUserDTO>(HttpStatusCode.BadRequest, "Password must contain 4 symbols or more");

        if (!Regex.IsMatch(createUser.Password, @"^[a-zA-Z0-9]+$"))
            return new Response<GetUserDTO>(HttpStatusCode.BadRequest, "Password must only contain letters and digits! (no symbols or spaces)");

        //Role
        var validRoles = new[] { "User", "Admin" };
        if (!validRoles.Contains(createUser.Role))
            return new Response<GetUserDTO>(HttpStatusCode.BadRequest, "Invalid role, only User and Admin roles accepted");

        var user = mapper.Map<User>(createUser);
        user.PasswordHash = passwordHasher.HashPassword(user, createUser.Password);
        user.IsEmailVerified = true;

        if (await userRepository.AddAsync(user) == 0)
            return new Response<GetUserDTO>(HttpStatusCode.InternalServerError, "User registration error!");

        var emailDto = new EmailDTO
        {
            To = user.Email,
            Subject = "Registration info",
            Body = $"Hello {user.FirstName},\n\nYour account has been registegred successfully"
        };

        await emailService.SendEmailAsync(emailDto);

        var getUserDTO = mapper.Map<GetUserDTO>(user);

        return new Response<GetUserDTO>(getUserDTO);
    }

    public async Task<Response<GetUserDTO>> UpdateAsync(ClaimsPrincipal userClaims, int id, UpdateUserDTO updateUser)
    {
        updateUser.FirstName = updateUser.FirstName.Trim();
        updateUser.LastName = updateUser.LastName.Trim();
        updateUser.Email = updateUser.Email.Trim();
        updateUser.Phone = updateUser.Phone.Trim();

        var userIdStr = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
            return new Response<GetUserDTO>(HttpStatusCode.Unauthorized, "Invalid user identity");

        //Id
        var checkUser = await repository.GetByIdAsync(id);
        if (checkUser == null || checkUser.IsDeleted == true)
            return new Response<GetUserDTO>(HttpStatusCode.NotFound, $"User with id {id} is not found");
        if (checkUser.Role == Roles.Admin && userId != id)
            return new Response<GetUserDTO>(HttpStatusCode.Forbidden, $"Access denied. Only admin himself can update his account");

        //FirstName
        if (string.IsNullOrWhiteSpace(updateUser.FirstName))
            return new Response<GetUserDTO>(HttpStatusCode.BadRequest, "First Name can't be empty");

        if (!updateUser.FirstName.All(char.IsLetterOrDigit))
            return new Response<GetUserDTO>(HttpStatusCode.BadRequest, "First Name must not contain spaces or special characters!");

        //LastName
        if (string.IsNullOrWhiteSpace(updateUser.LastName))
            return new Response<GetUserDTO>(HttpStatusCode.BadRequest, "Last Name can't be empty");

        if (!updateUser.LastName.All(char.IsLetterOrDigit))
            return new Response<GetUserDTO>(HttpStatusCode.BadRequest, "Last Name must not contain spaces or special characters!");

        //Phone
        if (string.IsNullOrWhiteSpace(updateUser.Phone))
            return new Response<GetUserDTO>(HttpStatusCode.BadRequest, "Phone can't be empty");

        if (!updateUser.Phone.All(char.IsDigit))
            return new Response<GetUserDTO>(HttpStatusCode.BadRequest, "Phone must contain only numbers, no spaces or special characters allowed!");

        if (updateUser.Phone.Length < 9)
            return new Response<GetUserDTO>(HttpStatusCode.BadRequest, "Phone size must contain at least 9 digits");

        if (!string.Equals(updateUser.Phone, checkUser.Phone, StringComparison.OrdinalIgnoreCase))
        {
            var phoneCheck = await userRepository.GetByPhoneAsync(updateUser.Phone);
            if (phoneCheck != null)
                return new Response<GetUserDTO>(HttpStatusCode.BadRequest, "User with this phone number is already registered");
        }

        //Email
        if (string.IsNullOrWhiteSpace(updateUser.Email))
            return new Response<GetUserDTO>(HttpStatusCode.BadRequest, "Email can't be empty");

        try
        {
            var addr = new System.Net.Mail.MailAddress(updateUser.Email);
            if (addr.Address != updateUser.Email)
                return new Response<GetUserDTO>(HttpStatusCode.BadRequest, "Email format is invalid");
        }
        catch
        {
            return new Response<GetUserDTO>(HttpStatusCode.BadRequest, "Email format is invalid");
        }

        if (!emailVerification.EmailVerificationMethod(updateUser.Email))
            return new Response<GetUserDTO>(HttpStatusCode.BadRequest, "Email domen is invalid.");

        if (!string.Equals(updateUser.Email, checkUser.Email, StringComparison.OrdinalIgnoreCase))
        {
            var emailCheck = await userRepository.GetByEmailAsync(updateUser.Email);
            if (emailCheck != null)
                return new Response<GetUserDTO>(HttpStatusCode.BadRequest, "User with this email is already registered");
        }

        mapper.Map(updateUser, checkUser);

        if (await userRepository.UpdateAsync(checkUser) == 0)
            return new Response<GetUserDTO>(HttpStatusCode.InternalServerError, "User update error!");

        var emailDto = new EmailDTO
        {
            To = checkUser.Email,
            Subject = "Account info",
            Body = $"Hello {checkUser.FirstName},\n\nYour account has been updated successfully"
        };

        await emailService.SendEmailAsync(emailDto);

        var getUserDTO = mapper.Map<GetUserDTO>(checkUser);

        return new Response<GetUserDTO>(getUserDTO);
    }

    public async Task<Response<string>> DeleteAsync(int userId)
    {
        var user = await userRepository.GetByIdAsync(userId);
        if (user == null)
            return new Response<string>(HttpStatusCode.NotFound, $"User with id {userId} is not found");

        if (await userRepository.DeleteAsync(user) == 0)
            return new Response<string>(HttpStatusCode.InternalServerError, "User couldn't be deleted");

        var emailDto = new EmailDTO
        {
            To = user.Email,
            Subject = "Account info",
            Body = $"Hello {user.FirstName},\n\nYour account has been deleted successfully"
        };
        await emailService.SendEmailAsync(emailDto);

        return new Response<string>("User deleted successfully");
    }

    public async Task<Response<string>> DeleteSelfAsync(ClaimsPrincipal userClaims)
    {
        var userIdStr = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
            return new Response<string>(HttpStatusCode.Unauthorized, "Invalid user identity");

        var user = await userRepository.GetByIdAsync(userId);
        if (user == null || user.IsDeleted)
            return new Response<string>(HttpStatusCode.NotFound, $"User with id {userId} not found");

        if (await userRepository.MarkUserAsDeleted(userId) == 0)
            return new Response<string>(HttpStatusCode.InternalServerError, "User couldn't be deleted");

        var emailDto = new EmailDTO
        {
            To = user.Email,
            Subject = "Account info",
            Body = $"Hello {user.FirstName},\n\nYour account has been deleted successfully"
        };
        await emailService.SendEmailAsync(emailDto);

        return new Response<string>("User deleted successfully");
    }

    public async Task<Response<GetUserDTO>> GetByIdAsync(int userId)
    {
        var user = await userRepository.GetByIdAsync(userId);
        if (user == null)
            return new Response<GetUserDTO>(HttpStatusCode.NotFound, $"User with id {userId} is not found");

        var getUserDto = mapper.Map<GetUserDTO>(user);

        var request = httpContextAccessor.HttpContext?.Request;
        if (!string.IsNullOrEmpty(user.ProfileImagePath) && request != null)
        {
            getUserDto.ProfileImageUrl = $"{request.Scheme}://{request.Host}{user.ProfileImagePath}";
        }

        return new Response<GetUserDTO>(getUserDto);
    }

    public async Task<Response<List<GetUserDTO>>> GetByNameAsync(string name)
    {
        var users = await userRepository.GetByUserNameAsync(name);
        if (users == null)
            return new Response<List<GetUserDTO>>(HttpStatusCode.NotFound, $"No matching users");

        var getUserDto = mapper.Map<List<GetUserDTO>>(users);

        var request = httpContextAccessor.HttpContext?.Request;
        for (int i = 0; i < users.Count; i++)
        {
            if (!string.IsNullOrEmpty(users[i].ProfileImagePath) && request != null)
            {
                getUserDto[i].ProfileImageUrl = $"{request.Scheme}://{request.Host}{users[i].ProfileImagePath}";
            }
        }

        return new Response<List<GetUserDTO>>(getUserDto);
    }

    public async Task<Response<GetUserDTO>> GetCurrentUserAsync(ClaimsPrincipal userClaims)
    {
        var userIdClaim = userClaims.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return new Response<GetUserDTO>(HttpStatusCode.Unauthorized, "User ID not found in token");

        if (!int.TryParse(userIdClaim.Value, out int userId))
            return new Response<GetUserDTO>(HttpStatusCode.BadRequest, "Invalid user ID in token");

        var user = await userRepository.GetByIdAsync(userId);
        if (user == null || user.IsDeleted)
            return new Response<GetUserDTO>(HttpStatusCode.NotFound, "User not found");

        var getUserDto = mapper.Map<GetUserDTO>(user);

        var request = httpContextAccessor.HttpContext?.Request;
        if (!string.IsNullOrEmpty(user.ProfileImagePath) && request != null)
        {
            getUserDto.ProfileImageUrl = $"{request.Scheme}://{request.Host}{user.ProfileImagePath}";
        }
        
        return new Response<GetUserDTO>(getUserDto);
    }

    public async Task<Response<List<GetUserDTO>>> GetAllAsync(UserFilter filter)
    {
        var query = repository.GetAll();

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            var search = filter.Name.ToLower();
            query = query.Where(d =>
                d.FirstName.ToLower().Contains(search) ||
                d.LastName.ToLower().Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(filter.Phone))
            query = query.Where(u => u.Phone.Contains(filter.Phone));

        if (!string.IsNullOrWhiteSpace(filter.Email))
            query = query.Where(u => u.Email.ToLower().Contains(filter.Email.ToLower()));

        if (!string.IsNullOrWhiteSpace(filter.Role))
            query = query.Where(u => u.Role.ToLower() == filter.Role.ToLower());

        if (filter.IsDeleted.HasValue)
            query = query.Where(u => u.IsDeleted == filter.IsDeleted.Value);

        if (filter.IsEmailVerified.HasValue)
            query = query.Where(u => u.IsEmailVerified == filter.IsEmailVerified.Value);

        var users = await query.ToListAsync();
        var getUsersDto = mapper.Map<List<GetUserDTO>>(users);

        var request = httpContextAccessor.HttpContext?.Request;
        if (request != null)
        {
            var baseUrl = $"{request.Scheme}://{request.Host}";

            foreach (var dto in getUsersDto)
            {
                var user = users.FirstOrDefault(u => u.Id == dto.Id);
                if (user != null && !string.IsNullOrEmpty(user.ProfileImagePath))
                {
                    dto.ProfileImageUrl = $"{baseUrl}{user.ProfileImagePath}";
                }
            }
        }

        return new Response<List<GetUserDTO>>(getUsersDto);
    }

    public async Task<Response<string>> RestoreAsync(RestoreUserDTO restoreUser)
    {
        restoreUser.Email = restoreUser.Email.Trim().ToLower();

        if (string.IsNullOrWhiteSpace(restoreUser.Email) || string.IsNullOrWhiteSpace(restoreUser.Password))
            return new Response<string>(HttpStatusCode.BadRequest, "Email and password are required");

        try
        {
            var addr = new System.Net.Mail.MailAddress(restoreUser.Email);
            if (addr.Address != restoreUser.Email)
                return new Response<string>(HttpStatusCode.BadRequest, "Email format is invalid");
        }
        catch
        {
            return new Response<string>(HttpStatusCode.BadRequest, "Email format is invalid");
        }

        // User
        var user = await userRepository.GetByEmailAsync(restoreUser.Email);
        if (user != null)
        {
            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, restoreUser.Password);
            if (result == PasswordVerificationResult.Failed)
                return new Response<string>(HttpStatusCode.BadRequest, "Invalid email or password");

            if (!user.IsDeleted)
                return new Response<string>(HttpStatusCode.BadRequest, "User is already active");

            user.IsDeleted = false;

            var updateResult = await repository.UpdateAsync(user);
            if (updateResult == 0)
                return new Response<string>(HttpStatusCode.InternalServerError, "Failed to restore user");

            await emailService.SendEmailAsync(new EmailDTO
            {
                To = user.Email,
                Subject = "Account Restored",
                Body = $"Hi {user.FirstName}, your account has been successfully restored!"
            });

            var getUserDto = mapper.Map<GetUserDTO>(user);

            return new Response<string>("Account restored successfully");
        }

        // Doctor
        var doctor = await doctorRepository.GetByEmailAsync(restoreUser.Email);
        if (doctor != null)
        {
            var result = passwordHasherDoctor.VerifyHashedPassword(doctor, doctor.PasswordHash, restoreUser.Password);
            if (result == PasswordVerificationResult.Failed)
                return new Response<string>(HttpStatusCode.BadRequest, "Invalid email or password");

            if (!doctor.IsDeleted)
                return new Response<string>(HttpStatusCode.BadRequest, "Doctor is already active");

            doctor.IsDeleted = false;

            var updateResult = await doctorRepository.UpdateAsync(doctor);
            if (updateResult == 0)
                return new Response<string>(HttpStatusCode.InternalServerError, "Failed to restore doctor");

            await emailService.SendEmailAsync(new EmailDTO
            {
                To = doctor.Email,
                Subject = "Account Restored",
                Body = $"Hi {doctor.FirstName}, your doctor account has been successfully restored!"
            });

            return new Response<string>("Doctor account restored successfully");
        }

        return new Response<string>(HttpStatusCode.NotFound, "Invalid email or password");
    }

    public async Task<Response<GetUserDTO>> ChangeUserRoleAsync(ChangeUserRoleDTO dto)
    {
        var user = await userRepository.GetByIdAsync(dto.UserId);
        if (user == null || user.IsDeleted)
            return new Response<GetUserDTO>(HttpStatusCode.NotFound, $"User with id {dto.UserId} not found");

        var validRoles = new[] { "User", "Admin" };
        if (!validRoles.Contains(dto.Role))
            return new Response<GetUserDTO>(HttpStatusCode.BadRequest, "Invalid role, only User and Admin roles accepted");

        if (user.Role == dto.Role)
            return new Response<GetUserDTO>(HttpStatusCode.BadRequest, $"User already assigned to {dto.Role} role");

        user.Role = dto.Role;

        if (await userRepository.UpdateAsync(user) == 0)
            return new Response<GetUserDTO>(HttpStatusCode.InternalServerError, "Failed to update user role");

        var emailDto = new EmailDTO
        {
            To = user.Email,
            Subject = "Account Role Updated",
            Body = $"Hello {user.FirstName}, your role has been changed to {user.Role}."
        };
        await emailService.SendEmailAsync(emailDto);

        var result = mapper.Map<GetUserDTO>(user);
        return new Response<GetUserDTO>(result);
    }

    public async Task<Response<string>> UploadOrUpdateProfileImageAsync(ClaimsPrincipal userClaims, IFormFile file)
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
        var userIdClaim = userClaims.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            return new Response<string>(HttpStatusCode.Unauthorized, "User ID not found or invalid in token.");

        var user = await userRepository.GetByIdAsync(userId);
        if (user == null || user.IsDeleted)
            return new Response<string>(HttpStatusCode.NotFound, $"User with ID {userId} not found.");

        // Deleting old image
        if (!string.IsNullOrEmpty(user.ProfileImagePath))
        {
            var oldFilePath = Path.Combine("wwwroot", user.ProfileImagePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
            if (File.Exists(oldFilePath))
                File.Delete(oldFilePath);
        }

        var folderPath = Path.Combine("wwwroot", "profile-images", "users");
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(folderPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        user.ProfileImagePath = $"/profile-images/users/{fileName}";
        await userRepository.UpdateAsync(user);

        return new Response<string>("Profile image uploaded successfully.");
    }

    public async Task<Response<string>> DeleteProfileImageAsync(ClaimsPrincipal userClaims)
    {
        var userIdClaim = userClaims.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            return new Response<string>(HttpStatusCode.Unauthorized, "User ID not found or invalid in token.");

        var user = await userRepository.GetByIdAsync(userId);
        if (user == null || user.IsDeleted)
            return new Response<string>(HttpStatusCode.NotFound, "User not found.");

        if (string.IsNullOrEmpty(user.ProfileImagePath))
            return new Response<string>(HttpStatusCode.BadRequest, "No profile image to delete.");

        var filePath = Path.Combine("wwwroot", user.ProfileImagePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
        if (File.Exists(filePath))
            File.Delete(filePath);

        user.ProfileImagePath = null;
        await userRepository.UpdateAsync(user);

        return new Response<string>("Profile image deleted successfully.");
    }

}