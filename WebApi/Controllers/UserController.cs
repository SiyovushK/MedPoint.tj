using Domain.Constants;
using Domain.DTOs.UserDTOs;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Data.ProfileImage;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<Response<GetUserDTO>>> CreateAsync(CreateUserDTO createUser)
    {
        var response = await userService.CreateAsync(createUser);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpPut]
    [Authorize]
    public async Task<ActionResult<Response<GetUserDTO>>> UpdateAsync(int id, UpdateUserDTO updateUser)
    {
        var response = await userService.UpdateAsync(id, updateUser);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpDelete]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<Response<string>>> DeleteAsync(int userId)
    {
        var response = await userService.DeleteAsync(userId);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpDelete("DeleteSelf")]
    [Authorize]
    public async Task<ActionResult<Response<string>>> DeleteSelfAsync()
    {
        var response = await userService.DeleteSelfAsync(User);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpGet("CurrentUser")]
    [Authorize]
    public async Task<ActionResult<Response<GetUserDTO>>> GetCurrentUserAsync()
    {
        var response = await userService.GetCurrentUserAsync(User);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpGet("ById")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<Response<GetUserDTO>>> GetByIdAsync(int userId)
    {
        var response = await userService.GetByIdAsync(userId);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpGet("All")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<Response<List<GetUserDTO>>>> GetAllAsync([FromQuery] UserFilter filter)
    {
        var response = await userService.GetAllAsync(filter);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpPost("RestoreAccount")]
    public async Task<ActionResult<Response<GetUserDTO>>> RestoreAsync(RestoreUserDTO restoreUser)
    {
        var response = await userService.RestoreAsync(restoreUser);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpPut("ChangeRole")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<Response<GetUserDTO>>> ChangeUserRoleAsync(ChangeUserRoleDTO dto)
    {
        var response = await userService.ChangeUserRoleAsync(dto);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpPost("{userId}/upload-or-update-profile-image")]
    public async Task<IActionResult> UploadProfileImage(int userId, IFormFile file)
    {
        var result = await userService.UploadOrUpdateProfileImageAsync(userId, file);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpDelete("{userId}/delete-profile-image")]
    public async Task<IActionResult> DeleteProfileImage(int userId)
    {
        var result = await userService.DeleteProfileImageAsync(userId);
        return StatusCode((int)result.StatusCode, result);
    }
}