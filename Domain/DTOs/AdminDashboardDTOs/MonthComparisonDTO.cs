namespace Domain.DTOs.AdminDashboardDTOs;

public class MonthComparisonDTO
{
    public string Category { get; set; } = string.Empty;
    public int Current { get; set; }
    public int Previous { get; set; }
    public double PercenteDifference { get; set; }
}