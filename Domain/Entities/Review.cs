using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Review
{
    [Key]
    public int Id { get; set; }
    public int? DoctorId { get; set; }
    [Required]
    public int UserId { get; set; }
    [Required]
    public int Rating { get; set; }
    [Required, StringLength(500)]
    public string Comment { get; set; } = string.Empty;

    public bool IsHidden { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }

    public Doctor? Doctor { get; set; }
    public User User { get; set; } = null!;
}