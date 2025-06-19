using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
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
    EmailVerification verification = new();

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

    public async Task<Response<string>> Register(RegisterDTO registerDto)
    {
        if (registerDto.FirstName == null || string.IsNullOrWhiteSpace(registerDto.FirstName))
            return new Response<string>(HttpStatusCode.BadRequest, $"First Name can't be empty!");

        if (registerDto.LastName == null || string.IsNullOrWhiteSpace(registerDto.LastName))
            return new Response<string>(HttpStatusCode.BadRequest, $"Last Name can't be empty!");

        var verificationResult = verification.EmailVerificationMethod(registerDto.Email);
        if (verificationResult == false)
            return new Response<string>(HttpStatusCode.BadRequest, $"Email input error!");

        if (registerDto.Phone == null || string.IsNullOrWhiteSpace(registerDto.Phone))
            return new Response<string>(HttpStatusCode.BadRequest, $"Phone can't be empty!");

        if (registerDto.Password.Length < 4 || string.IsNullOrWhiteSpace(registerDto.Password))
            return new Response<string>(HttpStatusCode.BadRequest, $"Password has to be 4 characters or more!");

        var existingUser = await userRepository.AnyAsync(c => c.Email.ToLower() == registerDto.Email.ToLower());
        if (existingUser)
            return new Response<string>(HttpStatusCode.BadRequest, "User with this email already exists.");

        var user = new User
        {  
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Email = registerDto.Email,  
            Phone = registerDto.Phone,  
            Role = Roles.User
        };
   
        user.PasswordHash = passwordHasher.HashPassword(user, registerDto.Password);

        var emailDto = new EmailDTO()
        {
            To = user.Email,
            Subject = "Account info",
            Body = $"Hi {user.FirstName}! Your registration has been successfull."
        };

        await emailService.SendEmailAsync(emailDto);

        await userRepository.AddAsync(user);
        await userRepository.SaveChangesAsync();

        return new Response<string>("User registered successfully");
    }

    public async Task<Response<string>> RequestResetPassword(RequestResetPasswordDTO requestResetPassword)
    {
        var user = await userRepository.FirstOrDefaultAsync(c => c.Email == requestResetPassword.Email);
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
        if ((resetPasswordDto.NewPassword.Length < 4 || string.IsNullOrWhiteSpace(resetPasswordDto.NewPassword)) &&
            (resetPasswordDto.ConfirmPassword.Length < 4 || string.IsNullOrWhiteSpace(resetPasswordDto.ConfirmPassword)))
            return new Response<string>(HttpStatusCode.BadRequest, $"Password has to be 4 characters or more!");

        if (resetPasswordDto.NewPassword != resetPasswordDto.ConfirmPassword)
            return new Response<string>(HttpStatusCode.BadRequest, $"Passwords don't match!");

        var user = await userRepository.FirstOrDefaultAsync(c => c.Email.ToLower() == resetPasswordDto.Email.ToLower());
        if (user == null)
        {
            return new Response<string>(HttpStatusCode.BadRequest, "Invalid token or email.");
        }

        if (string.IsNullOrEmpty(user.ResetToken) || user.ResetToken != resetPasswordDto.Token ||
            user.ResetTokenExpiry == null || user.ResetTokenExpiry < DateTime.UtcNow)
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