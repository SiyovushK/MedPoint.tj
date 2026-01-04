using Domain.Constants;
using Domain.DTOs.OrderDTOs;
using Domain.Filters;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController(IOrderService orderService) : ControllerBase
{   
    [HttpPost]
    [Authorize]
    public async Task<ActionResult> CreateAsync([FromBody] CreateOrderDTO createOrder)
        => Ok(await orderService.CreateAsync(User, createOrder));

    [HttpPost("By-admin")]
    [Authorize(Roles = $"{Roles.Doctor}, {Roles.Admin}")]
    public async Task<ActionResult> CreateByAdminAsync([FromBody] CreateOrderByAdminDTO createOrder)
        => Ok(await orderService.CreateByAdminAsync(createOrder));

    [HttpDelete]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult> DeleteAsync(int orderId)
        => Ok(await orderService.DeleteAsync(orderId));

    [HttpGet("ById")]
    [Authorize]
    public async Task<ActionResult> GetByIdAsync(int orderId)
        => Ok(await orderService.GetByIdAsync(orderId));

    [HttpGet("User-orders")]
    [Authorize(Roles = $"{Roles.User}, {Roles.Admin}")]
    public async Task<ActionResult> GetUserOrdersAsync(int userId)
        => Ok(await orderService.GetUserOrdersAsync(userId));

    [HttpGet("User-orders-filtered")]
    [Authorize(Roles = $"{Roles.User}, {Roles.Admin}")]
    public async Task<ActionResult> GetUserOrdersFilteredAsync([FromQuery] UserOrderFilter filter)
        => Ok(await orderService.GetUserOrdersFilteredAsync(filter));

    [HttpGet("Doctor-orders")]
    [Authorize(Roles = $"{Roles.Doctor}, {Roles.Admin}")]
    public async Task<ActionResult> GetDoctorOrdersAsync(int doctorId)
        => Ok(await orderService.GetDoctorOrdersAsync(doctorId));

    [HttpGet("Doctor-orders-availability")]
    [Authorize]
    public async Task<ActionResult> GetDoctorDaySchedule(int doctorId, DateOnly date)
        => Ok(await orderService.GetDoctorDaySchedule(doctorId, date));

    [HttpGet("Doctor-orders-filtered")]
    [Authorize(Roles = $"{Roles.Doctor}, {Roles.Admin}")]
    public async Task<ActionResult> GetDoctorOrdersFilteredAsync([FromQuery] DoctorOrderFilter filter)
        => Ok(await orderService.GetDoctorOrdersFilteredAsync(filter));

    [HttpGet("Doctor-orders-statistics")]
    [Authorize(Roles = $"{Roles.Doctor}, {Roles.Admin}")]
    public async Task<ActionResult> GetDoctorsOrderStatisticsAsync(int doctorId)
        => Ok(await orderService.GetDoctorsOrderStatisticsAsync(doctorId));

    [HttpGet("All")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult> GetAllAsync([FromQuery] OrderFilter filter)
        => Ok(await orderService.GetAllAsync(filter));
    
    [HttpPut("Confirm-order-by-doctor")]
    [Authorize(Roles = Roles.Doctor)]
    public async Task<ActionResult> ConfirmOrderAsync(int orderId)
        => Ok(await orderService.ConfirmOrderAsync(orderId, User));

    [HttpPut("Cancel-order-by-doctor")]
    [Authorize(Roles = Roles.Doctor)]
    public async Task<ActionResult> CancelOrderByDoctorAsync(CancelOrderDTO cancelOrder)
        => Ok(await orderService.CancelOrderByDoctorAsync(cancelOrder, User));

    [HttpPut("Cancel-order-by-user")]
    [Authorize(Roles = $"{Roles.User}, {Roles.Admin}")]
    public async Task<ActionResult> CancelOrderByUserAsync(int orderId)
        => Ok(await orderService.CancelOrderByUserAsync(orderId, User));
}