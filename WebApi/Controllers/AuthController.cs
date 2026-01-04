using Microsoft.AspNetCore.Mvc;
using Infrastructure.Interfaces;
using Domain.DTOs.AuthDTOs;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService _authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterDTO dto)
        => Ok(await _authService.Register(dto));

    [HttpPost("resend-verification-code")]
    public async Task<IActionResult> ResendVerificationCode([FromBody] string email)
        => Ok(await _authService.ResendVerificationCodeAsync(email));

    [HttpPost("verify-email")]
    public async Task<ActionResult> VerifyEmail([FromBody] VerifyEmailDTO dto)
        => Ok(await _authService.VerifyRegistrationCode(dto));

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginDTO dto)
        => Ok(await _authService.Login(dto));

    [HttpPost("request-password-reset")]
    public async Task<ActionResult> RequestPasswordReset([FromBody] RequestResetPasswordDTO dto)
        => Ok(await _authService.RequestResetPassword(dto));

    [HttpPost("reset-password")]
    [Authorize]
    public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)
        => Ok(await _authService.ResetPassword(dto));
}