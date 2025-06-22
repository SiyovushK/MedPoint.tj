using Domain.Enums;

namespace Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    public int UserId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public OrderStatus OrderStatus { get; set; } = OrderStatus.Active;

    public User User { get; set; } = null!;
    public Doctor Doctor { get; set; } = null!;
}