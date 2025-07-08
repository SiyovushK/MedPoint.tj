namespace Domain.DTOs.ScheduleDTOs;

public class GetDoctorScheduleDTO
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;

    public DayOfWeek DayOfWeek { get; set; }

    public TimeOnly WorkStart { get; set; }
    public TimeOnly WorkEnd { get; set; }

    public TimeOnly? LunchStart { get; set; }
    public TimeOnly? LunchEnd { get; set; }

    public bool IsDayOff { get; set; } = false;
}