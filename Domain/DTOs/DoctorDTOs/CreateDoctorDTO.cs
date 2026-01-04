using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs.DoctorDTOs;

public class CreateDoctorDTO : AbstractDoctor
{
    public List<DoctorEducation>? Education { get; set; }

    [Required(ErrorMessage = "Password is required!"), MinLength(4, ErrorMessage = "Password must be at least 4 characters long!")]
    [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Password must contain only letters and digits (no symbols or spaces).")]
    public string Password { get; set; }
}