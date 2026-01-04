using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs.AuthDTOs;

public class RequestResetPasswordDTO
{
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; }
}