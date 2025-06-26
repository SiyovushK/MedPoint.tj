namespace Domain.DTOs.ReviewDTOs;

public class CreateReviewDTO
{
    public int? DoctorId { get; set; }
    public int UserId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
}