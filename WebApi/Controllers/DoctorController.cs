using Domain.Constants;
using Domain.DTOs.DoctorDTOs;
using Domain.Filters;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoctorController(IDoctorService doctorService) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult> CreateAsync(CreateDoctorDTO createDoctor)
        => Ok(await doctorService.CreateAsync(createDoctor));

    [HttpPut]
    [Authorize(Roles = $"{Roles.Doctor}, {Roles.Admin}")]
    public async Task<ActionResult> UpdateAsync(int id, UpdateDoctorDTO updateDoctor)
        => Ok(await doctorService.UpdateAsync(id, updateDoctor));

    [HttpDelete]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult> DeleteAsync(int doctorId)
        => Ok(await doctorService.DeleteAsync(doctorId));

    [HttpPut("ChangeActivityStatus")]
    [Authorize(Roles = $"{Roles.Doctor}, {Roles.Admin}")]
    public async Task<ActionResult> ActivateOrDisableAsync(ChangeDoctorActivityStatus doctorActivity)
        => Ok(await doctorService.ActivateOrDisableAsync(doctorActivity));

    [HttpGet("ById")]
    [AllowAnonymous]
    public async Task<ActionResult> GetByIdAsync(int doctorId)
        => Ok(await doctorService.GetByIdAsync(doctorId));

    [HttpGet("ByName")]
    [AllowAnonymous]
    public async Task<ActionResult> GetByNameAsync(string name)
        => Ok(await doctorService.GetByNameAsync(name));

    [HttpGet("CurrentDoctor")]
    [Authorize(Roles = Roles.Doctor)]
    public async Task<ActionResult> GetCurrentDoctorAsync()
        => Ok(await doctorService.GetCurrentDoctorAsync(User));

    [HttpGet("All")]
    [AllowAnonymous]
    public async Task<ActionResult> GetAllAsync([FromQuery] DoctorFilter filter)
        => Ok(await doctorService.GetAllAsync(filter));

    [HttpGet("Doctor-statistics")]
    [Authorize(Roles = Roles.Doctor)]
    public async Task<ActionResult> GetDoctorStatisticsAsync()
        => Ok(await doctorService.GetDoctorStatisticsAsync(User));

    [HttpGet("Doctor-statistics-by-month")]
    [Authorize(Roles = Roles.Doctor)]
    public async Task<ActionResult> GetDoctorStatisticsByMonthAsync()
        => Ok(await doctorService.GetDoctorStatisticsByMonthAsync(User));

    [HttpGet("specializations")]
    [AllowAnonymous]
    public async Task<ActionResult> GetSpecializations()
        => Ok(await doctorService.GetSpecializationsAsync());

    [HttpPost("upload-or-update-profile-image")]
    [Authorize(Roles = Roles.Doctor)]
    public async Task<ActionResult> UploadOrUpdateProfileImageAsync(IFormFile file)
        => Ok(await doctorService.UploadOrUpdateProfileImageAsync(User, file));

    [HttpDelete("delete-profile-image")]
    [Authorize(Roles = Roles.Doctor)]
    public async Task<ActionResult> DeleteProfileImageAsync()
        => Ok(await doctorService.DeleteProfileImageAsync(User));
}