using Domain.Enums;

namespace Domain.DTOs.OrderDTOs;

public class GetOrderDTO
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    public DateTime CreatedAt { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public string? CancellationReason { get; set; }
}