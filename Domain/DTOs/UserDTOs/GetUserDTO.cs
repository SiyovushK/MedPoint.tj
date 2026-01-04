namespace Domain.DTOs.UserDTOs;

public class GetUserDTO : AbstractUser
{
    public int Id { get; set; }
    public string Role { get; set; }

    public string? ProfileImageUrl { get; set; }

    public bool IsDeleted { get; set; }
    public bool IsEmailVerified { get; set; }
    
    public DateTime CreatedAt { get; set; }
}