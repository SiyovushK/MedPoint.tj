namespace Domain.DTOs.OrderDTOs;

public class CreateOrderByAdminDTO
{
    public int DoctorId { get; set; }
    public int UserId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
}