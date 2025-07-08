namespace Domain.DTOs.ScheduleDTOs;

public class CreateDoctorScheduleDTO
{
    public int DoctorId { get; set; }

    public DayOfWeek DayOfWeek { get; set; }

    public TimeOnly? WorkStart { get; set; }
    public TimeOnly? WorkEnd { get; set; }

    public TimeOnly? LunchStart { get; set; }
    public TimeOnly? LunchEnd { get; set; }

    public bool IsDayOff { get; set; } = false;
}