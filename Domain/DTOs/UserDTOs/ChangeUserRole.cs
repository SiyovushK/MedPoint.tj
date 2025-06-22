using Domain.Constants;

namespace Domain.DTOs.UserDTOs;

public class ChangeUserRoleDTO
{
    public int UserId { get; set; }
    public string Role { get; set; } = string.Empty;
}