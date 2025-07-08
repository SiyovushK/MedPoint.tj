using Domain.Constants;
using Domain.DTOs.ScheduleDTOs;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = $"{Roles.Doctor}, {Roles.Admin}")]
public class DoctorScheduleController(IDoctorScheduleService scheduleService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Response<GetDoctorScheduleDTO>>> CreateAsync(CreateDoctorScheduleDTO dto)
    {
        var result = await scheduleService.CreateAsync(dto);
        return StatusCode((int)result.StatusCode, result); 
    }

    [HttpPut]
    public async Task<ActionResult<Response<GetDoctorScheduleDTO>>> UpdateAsync(int id, UpdateDoctorScheduleDTO dto)
    {
        var result = await scheduleService.UpdateAsync(id, dto);
        return StatusCode((int)result.StatusCode, result); 
    }

    [HttpDelete]
    public async Task<ActionResult<Response<string>>> DeleteAsync(int id)
    {
        var result = await scheduleService.DeleteAsync(id);
        return StatusCode((int)result.StatusCode, result); 
    }

    [HttpGet("ById")]
    public async Task<ActionResult<Response<GetDoctorScheduleDTO>>> GetByIdAsync(int id)
    {
        var result = await scheduleService.GetByIdAsync(id);
        return StatusCode((int)result.StatusCode, result); 
    }

    [HttpGet("By-doctor-id")]
    public async Task<ActionResult<Response<List<GetDoctorScheduleDTO>>>> GetByDoctorIdAsync(int doctorId)
    {
        var result = await scheduleService.GetByDoctorIdAsync(doctorId);
        return StatusCode((int)result.StatusCode, result); 
    }

    [HttpGet("All")]
    public async Task<ActionResult<Response<List<GetDoctorScheduleDTO>>>> GetAllAsync([FromQuery] DoctorScheduleFilter filter)
    {
        var result = await scheduleService.GetAllAsync(filter);
        return StatusCode((int)result.StatusCode, result); 
    }
}