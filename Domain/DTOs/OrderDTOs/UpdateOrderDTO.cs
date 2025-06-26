using Domain.Enums;

namespace Domain.DTOs.OrderDTOs;

public class UpdateOrderDTO
{
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    public OrderStatus OrderStatus { get; set; }
}