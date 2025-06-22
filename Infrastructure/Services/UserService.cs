using System.Net;
using System.Security.Claims;
using System.Text.RegularExpressions;
using AutoMapper;
using Domain.DTOs.EmailDTOs;
using Domain.DTOs.UserDTOs;
using Domain.Entities;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Infrastructure.Services.HelperServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class UserService(
        IBaseRepository<User, int> repository,
        UserRepository userRepository,
        IMapper mapper,
        IPasswordHasher<User> passwordHasher,
        IEmailService emailService) : IUserService
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
            return new Response<GetUserDTO>(HttpStatusCode.BadRequest, "Email must be from gmail.com or mail.ru");

        var emailCheck = await userRepository.GetByEmailAsync(createUser.Email.ToLower());
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

    public async Task<Response<GetUserDTO>> UpdateAsync(int id, UpdateUserDTO updateUser)
    {
        updateUser.FirstName = updateUser.FirstName.Trim();
        updateUser.LastName = updateUser.LastName.Trim();
        updateUser.Email = updateUser.Email.Trim();
        updateUser.Phone = updateUser.Phone.Trim();

        //Id
        var checkUser = await repository.GetByIdAsync(id);
        if (checkUser == null || checkUser.IsDeleted == true)
            return new Response<GetUserDTO>(HttpStatusCode.NotFound, $"User with id {id} is not found");

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
            return new Response<GetUserDTO>(HttpStatusCode.BadRequest, "Email must be from gmail.com or mail.ru");

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
        return new Response<GetUserDTO>(getUserDto);
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
        return new Response<GetUserDTO>(getUserDto);
    }

    public async Task<PagedResponse<List<GetUserDTO>>> GetAllAsync(UserFilter filter)
    {
        var pageNumber = filter.PageNumber;
        var pageSize = filter.PageSize;
        PaginationHelper.Normalize(ref pageNumber, ref pageSize);

        var query = repository.GetAll();

        if (filter.Id.HasValue)
            query = query.Where(u => u.Id == filter.Id.Value);

        if (!string.IsNullOrWhiteSpace(filter.FirstName))
            query = query.Where(u => u.FirstName.ToLower().Contains(filter.FirstName.ToLower()));

        if (!string.IsNullOrWhiteSpace(filter.LastName))
            query = query.Where(u => u.LastName.ToLower().Contains(filter.LastName.ToLower()));

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

        var totalRecords = await query.CountAsync();
        if (totalRecords == 0)
            return new PagedResponse<List<GetUserDTO>>(HttpStatusCode.NotFound, "No matched users");

        var pagedUsers = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var getUsersDto = mapper.Map<List<GetUserDTO>>(pagedUsers);

        return new PagedResponse<List<GetUserDTO>>(getUsersDto, pageSize, pageNumber, totalRecords);
    }

    public async Task<Response<GetUserDTO>> RestoreAsync(RestoreUserDTO restoreUser)
    {
        restoreUser.Email = restoreUser.Email.Trim();

        if (string.IsNullOrWhiteSpace(restoreUser.Email) || string.IsNullOrWhiteSpace(restoreUser.Password))
            return new Response<GetUserDTO>(HttpStatusCode.BadRequest, "Email and password are required");

        try
        {
            var addr = new System.Net.Mail.MailAddress(restoreUser.Email);
            if (addr.Address != restoreUser.Email)
                return new Response<GetUserDTO>(HttpStatusCode.BadRequest, "Email format is invalid");
        }
        catch
        {
            return new Response<GetUserDTO>(HttpStatusCode.BadRequest, "Email format is invalid");
        }

        var user = await userRepository.GetByEmailAsync(restoreUser.Email);
        if (user is null)
            return new Response<GetUserDTO>(HttpStatusCode.NotFound, "Invalid email or password");

        if (!user.IsDeleted)
            return new Response<GetUserDTO>(HttpStatusCode.BadRequest, "User is already active");

        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, restoreUser.Password);
        if (result == PasswordVerificationResult.Failed)
            return new Response<GetUserDTO>(HttpStatusCode.BadRequest, "Invalid email or password");

        user.IsDeleted = false;

        var updateResult = await repository.UpdateAsync(user);
        if (updateResult == 0)
            return new Response<GetUserDTO>(HttpStatusCode.InternalServerError, "Failed to restore user");

        await emailService.SendEmailAsync(new EmailDTO
        {
            To = user.Email,
            Subject = "Account Restored",
            Body = $"Hi {user.FirstName}, your account has been successfully restored!"
        });

        var getUserDto = mapper.Map<GetUserDTO>(user);
        return new Response<GetUserDTO>(getUserDto);
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

}