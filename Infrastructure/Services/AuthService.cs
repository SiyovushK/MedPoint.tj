using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Domain.Constants;
using Domain.DTOs.AuthDTOs;
using Domain.DTOs.EmailDTOs;
using Domain.Entities;
using Domain.Responses;
using Infrastructure.Interfaces;
using Infrastructure.Services.HelperServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

public class AuthService(
        IAuthRepository<User> userRepository,
        IPasswordHasher<User> passwordHasherUser,
        IAuthRepository<Doctor> doctorRepository,
        IPasswordHasher<Doctor> passwordHasherDoctor,
        IConfiguration config,
        IEmailService emailService) : IAuthService
{
    EmailVerification emailVerification = new();

    public async Task<Response<string>> Register(RegisterDTO registerDto)
    {
        registerDto.FirstName = registerDto.FirstName.Trim();
        registerDto.LastName = registerDto.LastName.Trim();
        registerDto.Email = registerDto.Email.Trim();
        registerDto.Phone = registerDto.Phone.Trim();

        //FirstName
        if (string.IsNullOrWhiteSpace(registerDto.FirstName))
            return new Response<string>(HttpStatusCode.BadRequest, "First Name can't be empty");

        if (!registerDto.FirstName.All(char.IsLetterOrDigit))
            return new Response<string>(HttpStatusCode.BadRequest, "First Name must not contain spaces or special characters!");

        //LastName
        if (string.IsNullOrWhiteSpace(registerDto.LastName))
            return new Response<string>(HttpStatusCode.BadRequest, "Last Name can't be empty");

        if (!registerDto.LastName.All(char.IsLetterOrDigit))
            return new Response<string>(HttpStatusCode.BadRequest, "Last Name must not contain spaces or special characters!");

        //Phone
        if (string.IsNullOrWhiteSpace(registerDto.Phone))
            return new Response<string>(HttpStatusCode.BadRequest, "Phone can't be empty");

        if (!registerDto.Phone.All(char.IsDigit))
            return new Response<string>(HttpStatusCode.BadRequest, "Phone must contain only numbers, no spaces or special characters allowed!");

        if (registerDto.Phone.Length < 9)
            return new Response<string>(HttpStatusCode.BadRequest, "Phone size must contain at least 9 digits");

        var phoneCheck = await userRepository.AnyAsync(c => c.Phone.Trim() == registerDto.Phone.Trim());
        if (phoneCheck)
            return new Response<string>(HttpStatusCode.BadRequest, "User with this phone numbers alredy registered");

        //Email
        if (string.IsNullOrWhiteSpace(registerDto.Email))
            return new Response<string>(HttpStatusCode.BadRequest, "Email can't be empty");

        try
        {
            var addr = new System.Net.Mail.MailAddress(registerDto.Email);
            if (addr.Address != registerDto.Email)
                return new Response<string>(HttpStatusCode.BadRequest, "Email format is invalid");
        }
        catch
        {
            return new Response<string>(HttpStatusCode.BadRequest, "Email format is invalid");
        }

        if (!emailVerification.EmailVerificationMethod(registerDto.Email))
            return new Response<string>(HttpStatusCode.BadRequest, "Email must be from gmail.com or mail.ru");

        var emailCheck = await userRepository.AnyAsync(c => c.Email.ToLower() == registerDto.Email.ToLower());
        if (emailCheck)
            return new Response<string>(HttpStatusCode.BadRequest, "User with this email is alredy registered");

        //Password
        if (string.IsNullOrWhiteSpace(registerDto.Password))
            return new Response<string>(HttpStatusCode.BadRequest, "Password can't be empty");

        if (registerDto.Password.Length < 6)
            return new Response<string>(HttpStatusCode.BadRequest, "Password must contain 4 symbols or more");

        if (!Regex.IsMatch(registerDto.Password, @"^[a-zA-Z0-9]+$"))
            return new Response<string>(HttpStatusCode.BadRequest, "Password must only contain letters and digits! (no symbols or spaces)");

        var code = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();

        var newUser = new User
        {
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Email = registerDto.Email,
            Phone = registerDto.Phone,
            Role = Roles.User,
            ResetToken = code,
            ResetTokenExpiry = DateTime.UtcNow.AddMinutes(10),
        };

        newUser.PasswordHash = passwordHasherUser.HashPassword(newUser, registerDto.Password);

        await userRepository.AddAsync(newUser);
        await userRepository.SaveChangesAsync();

        await emailService.SendEmailAsync(new EmailDTO
        {
            To = newUser.Email,
            Subject = "Registration Confirmation Code",
            Body = $"Hello {newUser.FirstName},\n\nYour confirmation code is: {code}"
        });

        return new Response<string>("Verification code sent to email.");
    }

    public async Task<Response<string>> VerifyRegistrationCode(VerifyEmailDTO dto)
    {
        var user = await userRepository.FirstOrDefaultAsync(u =>
            u.Email.ToLower() == dto.Email.Trim().ToLower() &&
            u.ResetToken == dto.Code);

        if (user == null)
            return new Response<string>(HttpStatusCode.BadRequest, "Invalid email or code");

        if (user.ResetTokenExpiry < DateTime.UtcNow)
            return new Response<string>(HttpStatusCode.BadRequest, "Verification code expired");

        user.ResetToken = null;
        user.ResetTokenExpiry = null;
        user.IsEmailVerified = true;

        userRepository.Update(user);
        await userRepository.SaveChangesAsync();

        return new Response<string>("Email verified. Registration completed.");
    }

    public async Task<Response<string>> ResendVerificationCodeAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return new Response<string>(HttpStatusCode.BadRequest, "Email is required.");

        var existingUser = await userRepository.FirstOrDefaultAsync(u =>
            u.Email.ToLower() == email.Trim().ToLower());

        if (existingUser == null)
            return new Response<string>(HttpStatusCode.NotFound, "User with this email does not exist.");

        if (existingUser.IsEmailVerified)
            return new Response<string>(HttpStatusCode.BadRequest, "This user is already verified.");

        var code = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();

        existingUser.ResetToken = code;
        existingUser.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(10);

        userRepository.Update(existingUser);
        await userRepository.SaveChangesAsync();

        await emailService.SendEmailAsync(new EmailDTO
        {
            To = existingUser.Email,
            Subject = "New Verification Code",
            Body = $"Hello {existingUser.FirstName},\n\nYour new verification code is: {code}"
        });

        return new Response<string>("New verification code sent to your email.");
    }

    public async Task<Response<TokenDTO>> Login(LoginDTO loginDto)
    {
        var email = loginDto.Email.Trim().ToLower();

        // 1. Проверка пользователей
        var user = await userRepository.FirstOrDefaultAsync(c => c.Email.ToLower() == email);
        if (user != null)
        {
            var passwordResult = passwordHasherUser.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);
            if (passwordResult == PasswordVerificationResult.Failed)
                return new Response<TokenDTO>(HttpStatusCode.BadRequest, "Incorrect email or password");

            if (!user.IsEmailVerified)
                return new Response<TokenDTO>(HttpStatusCode.Forbidden, "Email is not verified yet.");

            var token = GenerateJwt(user);

            return new Response<TokenDTO>(new TokenDTO { Token = token });
        }

        // 2. Проверка докторов
        var doctor = await doctorRepository.FirstOrDefaultAsync(d => d.Email.ToLower() == email);
        if (doctor != null)
        {
            var passwordResult = passwordHasherDoctor.VerifyHashedPassword(doctor, doctor.PasswordHash, loginDto.Password);
            if (passwordResult == PasswordVerificationResult.Failed)
                return new Response<TokenDTO>(HttpStatusCode.BadRequest, "Incorrect email or password");

            var token = GenerateJwt(doctor);

            return new Response<TokenDTO>(new TokenDTO { Token = token });
        }

        return new Response<TokenDTO>(HttpStatusCode.BadRequest, "Incorrect email or password");
    }

    public async Task<Response<string>> RequestResetPassword(RequestResetPasswordDTO dto)
    {
        var email = dto.Email.Trim().ToLower();
        if (string.IsNullOrWhiteSpace(email))
            return new Response<string>(HttpStatusCode.BadRequest, "Email is required.");

        // 1. Поиск среди пользователей
        var user = await userRepository.FirstOrDefaultAsync(c => c.Email.ToLower() == email);
        if (user != null)
        {
            var token = Guid.NewGuid().ToString();
            user.ResetToken = token;
            user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);
            userRepository.Update(user);
            await userRepository.SaveChangesAsync();

            await emailService.SendEmailAsync(new EmailDTO
            {
                To = email,
                Subject = "Password Reset Request",
                Body = $"Hi {user.FirstName},\n\nUse this token to reset your password:\n{token}"
            });

            return new Response<string>($"Reset token sent to: {email}");
        }

        // 2. Поиск среди докторов
        var doctor = await doctorRepository.FirstOrDefaultAsync(d => d.Email.ToLower() == email);
        if (doctor != null)
        {
            var token = Guid.NewGuid().ToString();
            doctor.ResetToken = token;
            doctor.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);
            doctorRepository.Update(doctor);
            await doctorRepository.SaveChangesAsync();

            await emailService.SendEmailAsync(new EmailDTO
            {
                To = email,
                Subject = "Password Reset Request",
                Body = $"Hi {doctor.FirstName},\n\nUse this token to reset your password:\n{token}"
            });

            return new Response<string>($"Reset token sent to: {email}");
        }

        return new Response<string>($"Email {email} is not registered.");
    }

    public async Task<Response<string>> ResetPassword(ResetPasswordDTO dto)
    {
        var email = dto.Email.Trim().ToLower();

        if (string.IsNullOrWhiteSpace(email))
            return new Response<string>(HttpStatusCode.BadRequest, "Email is required.");

        if (dto.NewPassword.Length < 4 || dto.ConfirmPassword.Length < 4)
            return new Response<string>(HttpStatusCode.BadRequest, $"Password has to be 4 characters or more!");

        if (!Regex.IsMatch(dto.NewPassword, @"^[a-zA-Z0-9]+$"))
            return new Response<string>(HttpStatusCode.BadRequest, "Password must contain only letters and digits");

        if (dto.NewPassword != dto.ConfirmPassword)
            return new Response<string>(HttpStatusCode.BadRequest, $"Passwords don't match!");

        // 1. Поиск в таблице User
        var user = await userRepository.FirstOrDefaultAsync(u => u.Email.ToLower() == email);
        if (user != null)
        {
            if (string.IsNullOrEmpty(user.ResetToken) || user.ResetTokenExpiry is null ||
                user.ResetToken != dto.Token || user.ResetTokenExpiry < DateTime.UtcNow)
                return new Response<string>(HttpStatusCode.BadRequest, "Invalid or expired token.");

            user.PasswordHash = passwordHasherUser.HashPassword(user, dto.NewPassword);
            user.ResetToken = null;
            user.ResetTokenExpiry = null;
            userRepository.Update(user);
            await userRepository.SaveChangesAsync();

            return new Response<string>("Password has been reset successfully.");
        }

        // 2. Поиск в таблице Doctor
        var doctor = await doctorRepository.FirstOrDefaultAsync(d => d.Email.ToLower() == email);
        if (doctor != null)
        {
            if (string.IsNullOrEmpty(doctor.ResetToken) || doctor.ResetTokenExpiry is null ||
                doctor.ResetToken != dto.Token || doctor.ResetTokenExpiry < DateTime.UtcNow)
                return new Response<string>(HttpStatusCode.BadRequest, "Invalid or expired token.");

            doctor.PasswordHash = passwordHasherDoctor.HashPassword(doctor, dto.NewPassword);
            doctor.ResetToken = null;
            doctor.ResetTokenExpiry = null;
            doctorRepository.Update(doctor);
            await doctorRepository.SaveChangesAsync();

            return new Response<string>("Password has been reset successfully.");
        }

        return new Response<string>(HttpStatusCode.BadRequest, "Invalid token or email.");
    }

    private string GenerateJwt(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

        if (!string.IsNullOrEmpty(user.Role))
        {
            claims.Add(new Claim(ClaimTypes.Role, user.Role));
        }

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    private string GenerateJwt(Doctor doctor)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, doctor.Id.ToString()),
            new Claim(ClaimTypes.Email, doctor.Email),
            new Claim(ClaimTypes.Role, Roles.Doctor)
        };

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}