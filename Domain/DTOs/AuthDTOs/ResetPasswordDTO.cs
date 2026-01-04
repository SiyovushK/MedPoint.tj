using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs.AuthDTOs;

public class ResetPasswordDTO
{
    public string Email { get; set; }
    public string Token { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [StringLength(255, ErrorMessage = "Must be between 4 and 255 characters", MinimumLength = 4)]
    [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Password must contain only letters and digits")]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }

    [Required(ErrorMessage = "Confirm Password is required")]
    [StringLength(255, ErrorMessage = "Must be between 4 and 255 characters", MinimumLength = 4)]
    [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Password must contain only letters and digits")]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; }
}