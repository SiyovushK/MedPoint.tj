namespace Domain.DTOs.AuthDTOs;

public class VerifyEmailDTO
{
    public string Email { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}