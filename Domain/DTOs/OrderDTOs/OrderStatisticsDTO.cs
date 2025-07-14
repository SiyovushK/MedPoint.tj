namespace Domain.DTOs.OrderDTOs;

public class OrderStatisticsDTO
{
    public string Month { get; set; } = string.Empty;
    public int Finished { get; set; }
    public int NotAccepted { get; set; }
    public int CancelledByUser { get; set; }
    public int CancelledByDoctor { get; set; }
}