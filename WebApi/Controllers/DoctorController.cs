using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoctorController(IDoctorService doctorService) : ControllerBase
{
    [HttpPost("{doctorId}/upload-profile-image")]
    public async Task<IActionResult> UploadProfileImage(int doctorId, IFormFile file)
    {
        var result = await doctorService.UploadOrUpdateProfileImageAsync(doctorId, file);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpDelete("{doctorId}/delete-profile-image")]
    public async Task<IActionResult> DeleteProfileImage(int doctorId)
    {
        var result = await doctorService.DeleteProfileImageAsync(doctorId);
        return StatusCode((int)result.StatusCode, result);
    }
}