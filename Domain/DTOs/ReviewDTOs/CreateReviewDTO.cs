using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs.ReviewDTOs;

public class CreateReviewDTO
{
    public int? DoctorId { get; set; }
    [Required]
    public int Rating { get; set; }
    [Required, StringLength(500)]
    public string Comment { get; set; } = string.Empty;
}