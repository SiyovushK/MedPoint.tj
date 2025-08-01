using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs.DoctorDTOs;

public class DoctorEducation
{
    [Key]
    public int Id { get; set; }
    public Int16 YearFrom { get; set; }
    public Int16? YearTo { get; set; }
    public string Description { get; set; } = string.Empty;
}