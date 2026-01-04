using Domain.Constants;
using Domain.DTOs.ScheduleDTOs;
using Domain.Filters;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoctorScheduleController(IDoctorScheduleService scheduleService) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = $"{Roles.Doctor}, {Roles.Admin}")]
    public async Task<ActionResult> CreateAsync(CreateDoctorScheduleDTO dto)
        => Ok(await scheduleService.CreateAsync(dto));

    [HttpPut]
    [Authorize(Roles = $"{Roles.Doctor}, {Roles.Admin}")]
    public async Task<ActionResult> UpdateAsync(int id, UpdateDoctorScheduleDTO dto)
        => Ok(await scheduleService.UpdateAsync(id, dto));

    [HttpDelete]
    [Authorize(Roles = $"{Roles.Doctor}, {Roles.Admin}")]
    public async Task<ActionResult> DeleteAsync(int id)
        => Ok(await scheduleService.DeleteAsync(id));

    [HttpGet("ById")]
    [Authorize(Roles = $"{Roles.Doctor}, {Roles.Admin}")]
    public async Task<ActionResult> GetByIdAsync(int id)
        => Ok(await scheduleService.GetByIdAsync(id));

    [HttpGet("By-doctor-id")]
    [AllowAnonymous]
    public async Task<ActionResult> GetByDoctorIdAsync(int doctorId)
        => Ok(await scheduleService.GetByDoctorIdAsync(doctorId));

    [HttpGet("All")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult> GetAllAsync([FromQuery] DoctorScheduleFilter filter)
        => Ok(await scheduleService.GetAllAsync(filter));
}