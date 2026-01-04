using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs.AuthDTOs;

public class RegisterDTO
{
    [Required(ErrorMessage = "First is required")]
    [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "First Name must not contain spaces or special characters!")]
    [StringLength(255, ErrorMessage = "Must be between 2 and 255 characters", MinimumLength = 2)]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last Name is required")]
    [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Last Name must not contain spaces or special characters!")]
    [StringLength(255, ErrorMessage = "Must be between 2 and 255 characters", MinimumLength = 2)]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format!")]
    [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
    public string Email { get; set; }

    [Required(ErrorMessage = "Phone number is required")]
    [MinLength(9, ErrorMessage = "Phone number must be at least 9 characters long!")]
    [RegularExpression("^[0-9]+$", ErrorMessage = "Phone number must contain only digits.")]
    public string Phone { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [StringLength(255, ErrorMessage = "Must be between 4 and 255 characters", MinimumLength = 4)]
    public string Password { get; set; }
}