using System.Security.Claims;
using Domain.Constants;
using Domain.DTOs.OrderDTOs;
using Domain.Filters;
using Domain.Responses;
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
    public async Task<ActionResult<Response<GetOrderDTO>>> CreateAsync([FromBody] CreateOrderDTO createOrder)
    {
        var result = await orderService.CreateAsync(createOrder);
        return StatusCode((int)result.StatusCode, result); 
    }

    [HttpDelete]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<Response<string>>> DeleteAsync(int orderId)
    {
        var result = await orderService.DeleteAsync(orderId);
        return StatusCode((int)result.StatusCode, result); 
    }

    [HttpGet("ById")]
    [Authorize]
    public async Task<ActionResult<Response<GetOrderDTO>>> GetByIdAsync(int orderId)
    {
        var result = await orderService.GetByIdAsync(orderId);
        return StatusCode((int)result.StatusCode, result); 
    }

    [HttpGet("User-orders")]
    [Authorize(Roles = $"{Roles.User}, {Roles.Admin}")]
    public async Task<ActionResult<Response<List<GetOrderDTO>>>> GetUserOrdersAsync(int userId)
    {
        var result = await orderService.GetUserOrdersAsync(userId);
        return StatusCode((int)result.StatusCode, result); 
    }

    [HttpGet("Doctor-orders")]
    [Authorize(Roles = $"{Roles.Doctor}, {Roles.Admin}")]
    public async Task<ActionResult<Response<List<GetOrderDTO>>>> GetDoctorOrdersAsync(int doctorId)
    {
        var result = await orderService.GetDoctorOrdersAsync(doctorId);
        return StatusCode((int)result.StatusCode, result); 
    }

    [HttpGet("All")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<Response<List<GetOrderDTO>>>> GetAllAsync([FromQuery] OrderFilter filter)
    {
        var result = await orderService.GetAllAsync(filter);
        return StatusCode((int)result.StatusCode, result); 
    }
    
    [HttpPut("Confirm-order-by-doctor")]
    [Authorize(Roles = Roles.Doctor)]
    public async Task<ActionResult<Response<GetOrderDTO>>> ConfirmOrderAsync(int orderId)
    {
        var result = await orderService.ConfirmOrderAsync(orderId, User);
        return StatusCode((int)result.StatusCode, result); 
    }

    [HttpPut("Cancel-order-by-doctor")]
    [Authorize(Roles = Roles.Doctor)]
    public async Task<ActionResult<Response<GetOrderDTO>>> CancelOrderByDoctorAsync(CancelOrderDTO cancelOrder)
    {
        var result = await orderService.CancelOrderByDoctorAsync(cancelOrder, User);
        return StatusCode((int)result.StatusCode, result); 
    }

    [HttpPut("Cancel-order-by-user")]
    [Authorize(Roles = $"{Roles.User}, {Roles.Admin}")]
    public async Task<ActionResult<Response<GetOrderDTO>>> CancelOrderByUserAsync(int orderId)
    {
        var result = await orderService.CancelOrderByUserAsync(orderId, User);
        return StatusCode((int)result.StatusCode, result); 
    }
}