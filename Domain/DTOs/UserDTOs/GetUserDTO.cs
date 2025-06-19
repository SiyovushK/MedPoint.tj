using Domain.Constants;

namespace Domain.DTOs.UserDTOs;

public class GetUserDTO
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = Roles.User;

    public bool IsDeleted { get; set; } = false;
}