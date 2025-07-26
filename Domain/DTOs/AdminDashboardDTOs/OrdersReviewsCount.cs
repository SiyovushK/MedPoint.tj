namespace Domain.DTOs.AdminDashboardDTOs;

public class OrdersReviewsCount
{
    public string Month { get; set; } = string.Empty;
    public int ReviewsCount { get; set; }
    public int OrdersCount { get; set; }
}