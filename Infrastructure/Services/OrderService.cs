using System.Security.Claims;
using Domain.DTOs.OrderDTOs;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Interfaces;

namespace Infrastructure.Services;

public class OrderService : IOrderService
{
    public Task<Response<List<GetOrderDTO>>> CancelOrderByDoctorAsync(CancelOrderDTO cancelOrder, ClaimsPrincipal doctorClaims)
    {
        throw new NotImplementedException();
    }

    public Task<Response<List<GetOrderDTO>>> CancelOrderByUserAsync(int orderId, ClaimsPrincipal userClaims)
    {
        throw new NotImplementedException();
    }

    public Task<Response<string>> ConfirmOrderAsync(int orderId, ClaimsPrincipal doctorClaims)
    {
        throw new NotImplementedException();
    }

    public Task<Response<GetOrderDTO>> CreateAsync(ClaimsPrincipal userClaims, CreateOrderDTO createOrder)
    {
        throw new NotImplementedException();
    }

    public Task<Response<string>> DeleteAsync(int orderId)
    {
        throw new NotImplementedException();
    }

    public Task<Response<List<GetOrderDTO>>> GetAllAsync(OrderFilter filter)
    {
        throw new NotImplementedException();
    }

    public Task<Response<GetOrderDTO>> GetByIdAsync(int orderId)
    {
        throw new NotImplementedException();
    }

    public Task<Response<List<GetOrderDTO>>> GetDoctorOrdersAsync(int doctorId)
    {
        throw new NotImplementedException();
    }

    public Task<Response<List<GetOrderDTO>>> GetUserOrdersAsync(int userId)
    {
        throw new NotImplementedException();
    }
}