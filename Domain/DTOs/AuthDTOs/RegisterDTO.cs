using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs.AuthDTOs;

public class RegisterDTO
{
    [Required(ErrorMessage = "First is required")]
    [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 2)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last Name is required")]
    [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 2)]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    [StringLength(9, ErrorMessage = "Must 9 characters", MinimumLength = 9)]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(255, ErrorMessage = "Must be between 4 and 255 characters", MinimumLength = 4)]
    public string Password { get; set; } = string.Empty;
}