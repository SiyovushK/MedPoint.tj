using Domain.Enums;

namespace Domain.DTOs.DoctorDTOs;

public class DoctorTimeTable
{
    public int DoctorId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }
    public OrderTimeStatus Status { get; set; }
}