using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs.ReviewDTOs;

public class CreateReviewDTO
{
    public int? DoctorId { get; set; }

    [Required(ErrorMessage = "Rating is required!")]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
    public int Rating { get; set; }
    
    [StringLength(500, ErrorMessage = "Comment cannot exceed 500 characters.")]
    public string Comment { get; set; }
}