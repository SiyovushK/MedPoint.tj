using Domain.Enums;

namespace Domain.DTOs.DoctorDTOs;

public class GetDoctorDTO
{
    public int Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Phone { get; set; }

    public string Email { get; set; }

    public string Description { get; set; }
    
    public List<DoctorEducation>? Education { get; set; }
    
    public DoctorSpecialization[] Specialization { get; set; } = [];

    public string? ProfileImageUrl { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }
}