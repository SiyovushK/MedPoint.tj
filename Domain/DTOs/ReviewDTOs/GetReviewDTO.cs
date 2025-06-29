namespace Domain.DTOs.ReviewDTOs;

public class GetReviewDTO
{
    public int Id { get; set; }
    public int? DoctorId { get; set; }
    public string? DoctorName { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}