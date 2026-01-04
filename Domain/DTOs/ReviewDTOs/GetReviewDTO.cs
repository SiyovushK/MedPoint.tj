namespace Domain.DTOs.ReviewDTOs;

public class GetReviewDTO
{
    public int Id { get; set; }
    public int? DoctorId { get; set; }
    public string? DoctorName { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; }
    public bool IsHidden { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}