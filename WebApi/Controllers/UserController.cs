using Domain.Constants;
using Domain.DTOs.UserDTOs;
using Domain.Filters;
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
    public async Task<ActionResult> CreateAsync(CreateUserDTO createUser)
        => Ok(await userService.CreateAsync(createUser));

    [HttpPut]
    [Authorize(Roles = $"{Roles.User}, {Roles.Admin}")]
    public async Task<ActionResult> UpdateAsync(int id, UpdateUserDTO updateUser)
        => Ok(await userService.UpdateAsync(User, id, updateUser));

    [HttpDelete]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult> DeleteAsync(int userId)
        => Ok(await userService.DeleteAsync(userId));

    [HttpDelete("DeleteSelf")]
    [Authorize(Roles = $"{Roles.User}, {Roles.Admin}")]
    public async Task<ActionResult> DeleteSelfAsync()
        => Ok(await userService.DeleteSelfAsync(User));

    [HttpGet("CurrentUser")]
    [Authorize(Roles = $"{Roles.User}, {Roles.Admin}")]
    public async Task<ActionResult> GetCurrentUserAsync()
        => Ok(await userService.GetCurrentUserAsync(User));

    [HttpGet("ById")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult> GetByIdAsync(int userId)
        => Ok(await userService.GetByIdAsync(userId));

    [HttpGet("ByName")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult> GetByNameAsync(string name)
        => Ok(await userService.GetByNameAsync(name));

    [HttpGet("All")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult> GetAllAsync([FromQuery] UserFilter filter)
        => Ok(await userService.GetAllAsync(filter));

    [HttpPost("RestoreAccount")]
    [AllowAnonymous]
    public async Task<ActionResult> RestoreAsync(RestoreUserDTO restoreUser)
        => Ok(await userService.RestoreAsync(restoreUser));

    [HttpPut("ChangeRole")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult> ChangeUserRoleAsync(ChangeUserRoleDTO dto)
        => Ok(await userService.ChangeUserRoleAsync(dto));

    [HttpPost("upload-or-update-profile-image")]
    [Authorize(Roles = $"{Roles.User}, {Roles.Admin}")]
    public async Task<ActionResult> UploadOrUpdateProfileImageAsync(IFormFile file)
        => Ok(await userService.UploadOrUpdateProfileImageAsync(User, file));

    [HttpDelete("delete-profile-image")]
    [Authorize(Roles = $"{Roles.User}, {Roles.Admin}")]
    public async Task<ActionResult> DeleteProfileImageAsync()
        => Ok(await userService.DeleteProfileImageAsync(User));
}