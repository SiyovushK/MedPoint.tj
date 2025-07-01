namespace Domain.DTOs.OrderDTOs;

public class CancelOrderDTO
{
    public int OrderId { get; set; }
    public string Reason { get; set; } = string.Empty;
}