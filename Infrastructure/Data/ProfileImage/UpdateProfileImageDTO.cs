using Microsoft.AspNetCore.Http;

namespace Infrastructure.Data.ProfileImage;

public class UpdateProfileImageDTO
{
    public IFormFile Image { get; set; } = null!;
}