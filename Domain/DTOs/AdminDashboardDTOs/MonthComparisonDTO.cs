namespace Domain.DTOs.AdminDashboardDTOs;

public class MonthComparisonDTO
{
    public string Category { get; set; }
    public int Current { get; set; }
    public int Previous { get; set; }
    public string PercenteDifference { get; set; }
}