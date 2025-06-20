using Domain.DTOs.EmailDTOs;

namespace Infrastructure.Interfaces;

public interface IEmailService
{
    Task<bool> SendEmailAsync(EmailDTO emailDto);
}