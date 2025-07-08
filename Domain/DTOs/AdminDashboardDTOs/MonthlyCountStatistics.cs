namespace Domain.DTOs.AdminDashboardDTOs;

public class MonthlyCountStatistics
{
    public string Month { get; set; } = string.Empty;
    public int DoctorsCount { get; set; }
    public int UsersCount { get; set; }
    public int ReviewsCount { get; set; }
    public int OrdersCount { get; set; }
}