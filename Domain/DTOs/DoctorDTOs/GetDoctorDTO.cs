using Domain.Enums;

namespace Domain.DTOs.DoctorDTOs;

public class GetDoctorDTO
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    
    public List<DoctorEducation>? Education { get; set; }
    
    public DoctorSpecialization[] Specialization { get; set; } = [];

    public string? ProfileImageUrl { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }
}