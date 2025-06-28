namespace Domain.DTOs.DoctorDTOs;

public class ChangeDoctorActivityStatus
{
    public int DoctorId { get; set; }
    public bool IsActive { get; set; }
}