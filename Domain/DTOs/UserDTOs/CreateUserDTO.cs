using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs.UserDTOs;

public class CreateUserDTO : AbstractUser
{
    [Required(ErrorMessage = "Password is required!")]
    [MinLength(4, ErrorMessage = "Password must be at least 4 characters long!")]
    [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Password must not contain spaces or special characters!")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}