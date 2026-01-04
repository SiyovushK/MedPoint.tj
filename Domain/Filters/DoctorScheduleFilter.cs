namespace Domain.Filters;

public class DoctorScheduleFilter
{
    public int? DoctorId { get; set; }
    public string? DoctorName { get; set; }

    public DayOfWeek? DayOfWeek { get; set; }

    public TimeOnly? WorkStart { get; set; }
    public TimeOnly? WorkEnd { get; set; }

    public bool? IsDayOff { get; set; } = false;
}