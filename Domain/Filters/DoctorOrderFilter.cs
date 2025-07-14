using Domain.Enums;

namespace Domain.Filters;

public class DoctorOrderFilter
{
    public int DoctorId { get; set; }
    public DateOnly? DateFrom { get; set; }
    public DateOnly? DateTo { get; set; }
    public OrderStatus? OrderStatus { get; set; }
}