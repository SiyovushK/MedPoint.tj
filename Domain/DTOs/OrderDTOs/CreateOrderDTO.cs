using Domain.Enums;

namespace Domain.DTOs.OrderDTOs;

public class CreateOrderDTO
{
    public int DoctorId { get; set; }
    public int UserId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}