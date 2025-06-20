using Domain.Constants;

namespace Domain.DTOs.UserDTOs;

public class ChangeUserRoleDTO
{
    public string Role { get; set; } = Roles.User;
}