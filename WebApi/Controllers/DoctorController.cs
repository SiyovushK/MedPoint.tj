using Domain.Constants;
using Domain.DTOs.DoctorDTOs;
using Domain.Filters;
using Domain.Responses;
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
    public async Task<ActionResult<Response<GetDoctorDTO>>> CreateAsync(CreateDoctorDTO createDoctor)
    {
        var result = await doctorService.CreateAsync(createDoctor);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPut]
    [Authorize(Roles = $"{Roles.Doctor}, {Roles.Admin}")]
    public async Task<ActionResult<Response<GetDoctorDTO>>> UpdateAsync(int id, UpdateDoctorDTO updateDoctor)
    {
        var result = await doctorService.UpdateAsync(id, updateDoctor);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpDelete]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<Response<string>>> DeleteAsync(int doctorId)
    {
        var result = await doctorService.DeleteAsync(doctorId);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPut("ChangeActivityStatus")]
    [Authorize(Roles = $"{Roles.Doctor}, {Roles.Admin}")]
    public async Task<ActionResult<Response<GetDoctorDTO>>> ActivateOrDisableAsync(ChangeDoctorActivityStatus doctorActivity)
    {
        var result = await doctorService.ActivateOrDisableAsync(doctorActivity);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("ById")]
    [AllowAnonymous]
    public async Task<ActionResult<Response<GetDoctorDTO>>> GetByIdAsync(int doctorId)
    {
        var result = await doctorService.GetByIdAsync(doctorId);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("ByName")]
    [AllowAnonymous]
    public async Task<ActionResult<Response<List<GetDoctorDTO>>>> GetByNameAsync(string name)
    {
        var result = await doctorService.GetByNameAsync(name);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("CurrentDoctor")]
    [Authorize(Roles = Roles.Doctor)]
    public async Task<ActionResult<Response<GetDoctorDTO>>> GetCurrentDoctorAsync()
    {
        var result = await doctorService.GetCurrentDoctorAsync(User);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("All")]
    [AllowAnonymous]
    public async Task<ActionResult<Response<List<GetDoctorDTO>>>> GetAllAsync([FromQuery] DoctorFilter filter)
    {
        var result = await doctorService.GetAllAsync(filter);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("Doctor-statistics")]
    [Authorize(Roles = Roles.Doctor)]
    public async Task<ActionResult<Response<DoctorStatisticsDTO>>> GetDoctorStatisticsAsync()
    {
        var result = await doctorService.GetDoctorStatisticsAsync(User);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("Doctor-statistics-by-month")]
    [Authorize(Roles = Roles.Doctor)]
    public async Task<ActionResult<Response<List<DoctorMonthlyStatistics>>>> GetDoctorStatisticsByMonthAsync()
    {
        var result = await doctorService.GetDoctorStatisticsByMonthAsync(User);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("specializations")]
    [AllowAnonymous]
    public async Task<ActionResult<Response<List<SpecializationDTO>>>> GetSpecializations()
    {
        var result = await doctorService.GetSpecializationsAsync();
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPost("upload-or-update-profile-image")]
    [Authorize(Roles = Roles.Doctor)]
    public async Task<ActionResult<Response<string>>> UploadOrUpdateProfileImageAsync(IFormFile file)
    {
        var result = await doctorService.UploadOrUpdateProfileImageAsync(User, file);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpDelete("delete-profile-image")]
    [Authorize(Roles = Roles.Doctor)]
    public async Task<ActionResult<Response<string>>> DeleteProfileImageAsync()
    {
        var result = await doctorService.DeleteProfileImageAsync(User);
        return StatusCode((int)result.StatusCode, result);
    }
}