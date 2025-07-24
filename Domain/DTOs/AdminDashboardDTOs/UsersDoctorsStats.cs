namespace Domain.DTOs.AdminDashboardDTOs;

public class UsersDoctorsStats
{
    public string Month { get; set; } = string.Empty;
    public int DoctorsCount { get; set; }
    public int UsersCount { get; set; }
}