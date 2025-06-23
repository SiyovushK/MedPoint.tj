using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Domain.DTOs.DoctorDTOs;

public class UpdateDoctorDTO
{
    [Required, StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required, StringLength(20)]
    public string Phone { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DoctorSpecialization[] Specialization { get; set; } = [];
}
