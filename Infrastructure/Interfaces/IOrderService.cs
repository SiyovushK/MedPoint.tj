using System.Security.Claims;
using Domain.DTOs.OrderDTOs;
using Domain.Filters;
using Domain.Responses;

namespace Infrastructure.Interfaces;

public interface IOrderService
{
    Task<Response<GetOrderDTO>> CreateAsync(ClaimsPrincipal userClaims, CreateOrderDTO createOrder);
    Task<Response<GetOrderDTO>> CreateByAdminAsync(CreateOrderByAdminDTO createOrder);
    Task<Response<string>> DeleteAsync(int orderId);
    Task<Response<GetOrderDTO>> GetByIdAsync(int orderId);
    Task<Response<List<GetOrderDTO>>> GetAllAsync(OrderFilter filter);

    Task<Response<GetOrderDTO>> ConfirmOrderAsync(int orderId, ClaimsPrincipal doctorClaims);
    Task<Response<GetOrderDTO>> CancelOrderByDoctorAsync(CancelOrderDTO cancelOrder, ClaimsPrincipal doctorClaims);
    Task<Response<GetOrderDTO>> CancelOrderByUserAsync(int orderId, ClaimsPrincipal userClaims);

    Task<Response<List<GetOrderDTO>>> GetUserOrdersAsync(int userId);
    Task<Response<List<GetOrderDTO>>> GetDoctorOrdersAsync(int doctorId);
}