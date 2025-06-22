namespace Domain.Entities;

public class Review
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    public int UserId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;

    public Doctor Doctor { get; set; } = null!;
    public User User { get; set; } = null!;
}