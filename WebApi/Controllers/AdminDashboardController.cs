using Domain.Constants;
using Domain.DTOs.AdminDashboardDTOs;
using Domain.DTOs.DoctorDTOs;
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
    [HttpGet("Count-stats")]
    public async Task<ActionResult<Response<CountStatisticsDTO>>> GetCountStatistics()
    {
        var result = await adminDashboard.GetCountStatistics();
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("Monthly-count-stats-Users-Doctors")]
    public async Task<ActionResult<Response<List<OrdersReviewsCount>>>> GetMonthlyCountStatisticsUsers()
    {
        var result = await adminDashboard.GetMonthlyCountStatisticsOrders();
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("Monthly-count-stats-Orders-Reviews")]
    public async Task<ActionResult<Response<List<UsersDoctorsStats>>>> GetMonthlyCountStatisticsOrders()
    {
        var result = await adminDashboard.GetMonthlyCountStatisticsUsers();
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("Popular-doctors")]
    public async Task<ActionResult<Response<List<PopularDoctorDTO>>>> GetPopularDoctors()
    { 
        var result = await adminDashboard.GetPopularDoctors();
        return StatusCode((int)result.StatusCode, result);
    }
}