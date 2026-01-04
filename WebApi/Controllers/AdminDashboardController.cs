using Domain.Constants;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Admin)]
public class AdminDashboardController(IAdminDashboardService adminDashboard) : ControllerBase
{
    [HttpGet("Count-stats")]
    public async Task<ActionResult> GetCountStatistics()
        => Ok(await adminDashboard.GetCountStatistics());

    [HttpGet("Monthly-count-stats-Users-Doctors")]
    public async Task<ActionResult> GetMonthlyCountStatisticsUsers()
        => Ok(await adminDashboard.GetMonthlyCountStatisticsUsers());

    [HttpGet("Monthly-count-stats-Orders-Reviews")]
    public async Task<ActionResult> GetMonthlyCountStatisticsOrders()
        => Ok(await adminDashboard.GetMonthlyCountStatisticsOrders());

    [HttpGet("Popular-doctors")]
    public async Task<ActionResult> GetPopularDoctors()
        => Ok(await adminDashboard.GetPopularDoctors());

    [HttpGet("Percentage-change-info")]
    public async Task<ActionResult> GetChangeByMonthInfoAsync()
        => Ok(await adminDashboard.GetChangeByMonthInfoAsync());
}