using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs.OrderDTOs;

public class CancelOrderDTO
{
    [Required]
    public int OrderId { get; set; }
    [Required]
    public string Reason { get; set; } = string.Empty;
}