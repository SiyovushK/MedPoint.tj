using Domain.Enums;

namespace Domain.Filters;

public class OrderFilter
{
    public int? DoctorId { get; set; }
    public string? DoctorName { get; set; }
    
    public int? UserId { get; set; }
    public string? UserName { get; set; }

    public DateOnly? DateFrom { get; set; }
    public DateOnly? DateTo { get; set; }

    public TimeOnly? StartTimeFrom { get; set; }
    public TimeOnly? StartTimeTo { get; set; }
    
    public DateOnly? CreatedFrom { get; set; }
    public DateOnly? CreatedTo { get; set; }

    public OrderStatus? OrderStatus { get; set; }
}