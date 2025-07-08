using System.ComponentModel.DataAnnotations;
using Domain.Constants;
using Domain.Enums;

namespace Domain.Entities;

public class Doctor
{
    public int Id { get; set; }
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
    public string Role { get; set; } = Roles.Doctor;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    public string? ProfileImagePath { get; set; }

    [Required]
    public DoctorSpecialization[] Specialization { get; set; } = [];

    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string? ResetToken { get; set; }
    public DateTime? ResetTokenExpiry { get; set; }

    public List<Review> Reviews { get; set; } = new();
    public List<Order> Orders { get; set; } = new();
    public List<DoctorSchedule> Schedules { get; set; } = new();
}