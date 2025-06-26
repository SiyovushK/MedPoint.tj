using Domain.Constants;
using Domain.DTOs.AdminDashboardDTOs;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Admin)]
public class AdminDashboardController(IAdminDashboardService adminDashboard) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<Response<CountStatisticsDTO>>> GetCountStatistics()
    {
        var result = await adminDashboard.GetCountStatistics();
        return StatusCode((int)result.StatusCode, result);
    }
}