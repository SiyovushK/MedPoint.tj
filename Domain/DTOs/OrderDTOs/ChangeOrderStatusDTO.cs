using Domain.Enums;

namespace Domain.DTOs.OrderDTOs;

public class ChangeOrderStatusDTO
{
    public int OrderId { get; set; }
    public OrderStatus OrderStatus { get; set; }
}