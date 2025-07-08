namespace Domain.Entities;

public class DoctorSchedule
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    public int DoctorName { get; set; }

    public DayOfWeek DayOfWeek { get; set; }

    public TimeOnly? WorkStart { get; set; }
    public TimeOnly? WorkEnd { get; set; }

    public TimeOnly? LunchStart { get; set; }
    public TimeOnly? LunchEnd { get; set; }

    public bool IsDayOff { get; set; } = false;

    public Doctor Doctor { get; set; } = null!;
}