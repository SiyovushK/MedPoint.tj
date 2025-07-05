namespace Domain.DTOs.DoctorDTOs;

public class PopularDoctorDTO
{
    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public int OrderCount { get; set; }
}