using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
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
        IPasswordHasher<User> passwordHasher,
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

        if (registerDto.Password.Length < 4)
            return new Response<string>(HttpStatusCode.BadRequest, "Password must contain 4 symbols or more");

        if (!Regex.IsMatch(registerDto.Password, @"^[a-zA-Z0-9]+$"))
            return new Response<string>(HttpStatusCode.BadRequest, "Password must only contain letters and digits! (no symbols or spaces)");

        var code = new Random().Next(100000, 999999).ToString();

        var user = new User
        {
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Email = registerDto.Email,
            Phone = registerDto.Phone,
            Role = Roles.User,
            ResetToken = code,
            ResetTokenExpiry = DateTime.UtcNow.AddMinutes(10),
        };

        user.PasswordHash = passwordHasher.HashPassword(user, registerDto.Password);

        await userRepository.AddAsync(user);
        await userRepository.SaveChangesAsync();

        var emailDto = new EmailDTO
        {
            To = user.Email,
            Subject = "Registration Confirmation Code",
            Body = $"Hello {user.FirstName},\n\nYour confirmation code is: {code}"
        };

        await emailService.SendEmailAsync(emailDto);

        return new Response<string>("Verification code sent to email");
    }

    public async Task<Response<string>> VerifyRegistrationCode(VerifyEmailDTO dto)
    {
        var user = await userRepository.FirstOrDefaultAsync(u =>
            u.Email.ToLower() == dto.Email.ToLower() &&
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

    public async Task<Response<TokenDTO>> Login(LoginDTO loginDto)
    {
        var user = await userRepository.FirstOrDefaultAsync(c => c.Email.ToLower() == loginDto.Email.ToLower());
        if (user == null)
        {
            return new Response<TokenDTO>(HttpStatusCode.BadRequest, "Incorrect email or password");
        }

        var passwordVerificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);
        if (passwordVerificationResult == PasswordVerificationResult.Failed)
        {
            return new Response<TokenDTO>(HttpStatusCode.BadRequest, "Incorrect email or password");
        }

        if (user.IsEmailVerified == false)
            return new Response<TokenDTO>(HttpStatusCode.Forbidden, "Email is not verified yet.");

        var emailDto = new EmailDTO()
        {
            To = user.Email,
            Subject = "Account info",
            Body = $"Hi {user.FirstName}! You have logged into your account successfully."
        };

        await emailService.SendEmailAsync(emailDto);

        var token = GenerateJwt(user);
        return new Response<TokenDTO>(new TokenDTO { Token = token });
    }

    public async Task<Response<string>> RequestResetPassword(RequestResetPasswordDTO requestResetPassword)
    {
        requestResetPassword.Email = requestResetPassword.Email.Trim().ToLower();
        
        if (string.IsNullOrWhiteSpace(requestResetPassword.Email))
            return new Response<string>(HttpStatusCode.BadRequest, "Email is required.");

        var user = await userRepository.FirstOrDefaultAsync(c => c.Email.ToLower() == requestResetPassword.Email);
        if (user == null)
        {
            return new Response<string>($"User with email: {requestResetPassword.Email} is not registered. Please procceed registration before password reset request.");
        }

        var resetToken = Guid.NewGuid().ToString();
        var tokenExpiry = DateTime.UtcNow.AddHours(1);

        user.ResetToken = resetToken;
        user.ResetTokenExpiry = tokenExpiry;

        userRepository.Update(user);
        await userRepository.SaveChangesAsync();

        var emailDto = new EmailDTO()
        {
            To = requestResetPassword.Email,
            Subject = "Password Reset Request",
            Body = $@"<p>Hello <strong>{user.FirstName}</strong>,</p>
                <p>You have requested a password reset. Please use the following token to reset your password:</p>
                <p><strong>{resetToken}</strong></p>
                <p>This token is valid for <strong>1 hour</strong>. If you did not request this, please ignore this email.</p>
                <p>Thank you,<br/>Your Support Team</p>"
        };

        var emailSent = await emailService.SendEmailAsync(emailDto);

        return emailSent
            ? new Response<string>($"A password reset link has been sent to email: {requestResetPassword.Email}.")
            : new Response<string>(HttpStatusCode.InternalServerError, "Failed to send reset password email.");
    }

    public async Task<Response<string>> ResetPassword(ResetPasswordDTO resetPasswordDto)
    {
        resetPasswordDto.Email = resetPasswordDto.Email.Trim().ToLower();

        if (string.IsNullOrWhiteSpace(resetPasswordDto.Email))
            return new Response<string>(HttpStatusCode.BadRequest, "Email is required.");

        if (resetPasswordDto.NewPassword.Length < 4 || resetPasswordDto.ConfirmPassword.Length < 4)
            return new Response<string>(HttpStatusCode.BadRequest, $"Password has to be 4 characters or more!");

        if (!Regex.IsMatch(resetPasswordDto.NewPassword, @"^[a-zA-Z0-9]+$"))
            return new Response<string>(HttpStatusCode.BadRequest, "Password must contain only letters and digits");

        if (resetPasswordDto.NewPassword != resetPasswordDto.ConfirmPassword)
            return new Response<string>(HttpStatusCode.BadRequest, $"Passwords don't match!");

        var user = await userRepository.FirstOrDefaultAsync(c => c.Email.ToLower() == resetPasswordDto.Email);
        if (user == null)
        {
            return new Response<string>(HttpStatusCode.BadRequest, "Invalid token or email.");
        }

        if (string.IsNullOrEmpty(user.ResetToken) || 
            user.ResetTokenExpiry is null || 
            user.ResetToken != resetPasswordDto.Token || 
            user.ResetTokenExpiry < DateTime.UtcNow)
        {
            return new Response<string>(HttpStatusCode.BadRequest, "Invalid or expired token.");
        }

        user.PasswordHash = passwordHasher.HashPassword(user, resetPasswordDto.NewPassword);

        user.ResetToken = null;
        user.ResetTokenExpiry = null;

        userRepository.Update(user);
        await userRepository.SaveChangesAsync();

        return new Response<string>("Password has been reset successfully.");
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
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}