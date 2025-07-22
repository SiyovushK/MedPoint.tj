namespace Domain.DTOs.DoctorDTOs;

public class DoctorStatisticsDTO
{
    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public int OrderCount { get; set; }
    public int ReviewCount { get; set; }
    public double AverageRating { get; set; }
}