using Domain.Enums;

namespace Domain.Filters;

public class UserOrderFilter
{
    public int UserId { get; set; }
    public DateOnly? DateFrom { get; set; }
    public DateOnly? DateTo { get; set; }
    public OrderStatus? OrderStatus { get; set; }
}