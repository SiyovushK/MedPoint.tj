using Domain.Enums;

namespace Domain.Filters;

public class DoctorFilter
{
    public string? Name { get; set; }
    public string? Email { get; set; }

    public DoctorSpecialization[]? Specializations { get; set; }

    public bool? IsActive { get; set; }
    public bool? IsDeleted { get; set; }
}