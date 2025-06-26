namespace Domain.Entities;

public class Review
{
    public int Id { get; set; }
    public int? DoctorId { get; set; }
    public int UserId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;

    public bool IsHidden { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public Doctor Doctor { get; set; } = null!;
    public User User { get; set; } = null!;
}