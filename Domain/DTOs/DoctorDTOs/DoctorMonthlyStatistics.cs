namespace Domain.DTOs.DoctorDTOs;

public class DoctorMonthlyStatistics
{
    public string Month { get; set; }
    public int DoctorId { get; set; }
    public string DoctorName { get; set; }
    public int OrderCount { get; set; }
    public int ReviewCount { get; set; }
}