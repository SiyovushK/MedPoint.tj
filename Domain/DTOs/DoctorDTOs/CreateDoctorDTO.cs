using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Domain.DTOs.DoctorDTOs;

public class CreateDoctorDTO
{
    [Required, StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required, StringLength(20)]
    public string Phone { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public List<DoctorEducation>? Education { get; set; }

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    public DoctorSpecialization[] Specialization { get; set; } = [];
}