using Microsoft.AspNetCore.Mvc;
using Domain.Responses;
using Infrastructure.Interfaces;
using Domain.DTOs.AuthDTOs;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService _authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<Response<string>>> Register([FromBody] RegisterDTO dto)
    {
        var response = await _authService.Register(dto);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpPost("verify-email")]
    public async Task<ActionResult<Response<string>>> VerifyEmail([FromBody] VerifyEmailDTO dto)
    {
        var response = await _authService.VerifyRegistrationCode(dto);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<Response<TokenDTO>>> Login([FromBody] LoginDTO dto)
    {
        var response = await _authService.Login(dto);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpPost("request-password-reset")]
    public async Task<ActionResult<Response<string>>> RequestPasswordReset([FromBody] RequestResetPasswordDTO dto)
    {
        var response = await _authService.RequestResetPassword(dto);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpPost("reset-password")]
    [Authorize]
    public async Task<ActionResult<Response<string>>> ResetPassword([FromBody] ResetPasswordDTO dto)
    {
        var response = await _authService.ResetPassword(dto);
        return StatusCode((int)response.StatusCode, response);
    }
}