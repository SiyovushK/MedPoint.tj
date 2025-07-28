namespace Domain.DTOs.DoctorDTOs;

public class DoctorMonthlyStatistics
{
    public string Month { get; set; } = string.Empty;
    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public int OrderCount { get; set; }
    public int ReviewCount { get; set; }
}