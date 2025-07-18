using Domain.Constants;
using Domain.DTOs.UserDTOs;
using Domain.Filters;
using Domain.Responses;
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
    [Authorize(Roles = $"{Roles.User}, {Roles.Admin}")]
    public async Task<ActionResult<Response<GetUserDTO>>> UpdateAsync(int id, UpdateUserDTO updateUser)
    {
        var response = await userService.UpdateAsync(User, id, updateUser);
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
    [Authorize(Roles = $"{Roles.User}, {Roles.Admin}")]
    public async Task<ActionResult<Response<string>>> DeleteSelfAsync()
    {
        var response = await userService.DeleteSelfAsync(User);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpGet("CurrentUser")]
    [Authorize(Roles = $"{Roles.User}, {Roles.Admin}")]
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

    [HttpGet("ByName")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<Response<List<GetUserDTO>>>> GetByNameAsync(string name)
    {
        var response = await userService.GetByNameAsync(name);
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
    [AllowAnonymous]
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

    [HttpPost("upload-or-update-profile-image")]
    [Authorize(Roles = $"{Roles.User}, {Roles.Admin}")]
    public async Task<ActionResult<Response<string>>> UploadOrUpdateProfileImageAsync(IFormFile file)
    {
        var result = await userService.UploadOrUpdateProfileImageAsync(User, file);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpDelete("delete-profile-image")]
    [Authorize(Roles = $"{Roles.User}, {Roles.Admin}")]
    public async Task<ActionResult<Response<string>>> DeleteProfileImageAsync()
    {
        var result = await userService.DeleteProfileImageAsync(User);
        return StatusCode((int)result.StatusCode, result);
    }
}