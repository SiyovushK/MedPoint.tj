using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Domain.DTOs.DoctorDTOs;

public class AbstractDoctor
{
    [Required(ErrorMessage = "First name is required!")]
    [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "First name must not contain spaces or special characters.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required!")]
    [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Last name must not contain spaces or special characters.")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Phone is required!"), MinLength(9, ErrorMessage = "Phone number must be at least 9 characters long!")]
    [RegularExpression("^[0-9]+$", ErrorMessage = "Phone number must contain only digits.")]
    public string Phone { get; set; }

    [Required(ErrorMessage = "Email is required!"), EmailAddress(ErrorMessage = "Invalid email format!")]
    public string Email { get; set; }

    public string Description { get; set; }

    [Required(ErrorMessage = "At least one specialization must be selected!")]
    public DoctorSpecialization[] Specialization { get; set; } = [];
}