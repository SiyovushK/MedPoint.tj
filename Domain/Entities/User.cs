using System.ComponentModel.DataAnnotations;
using Domain.Constants;

namespace Domain.Entities;

public class User
{
    [Key]
    public int Id { get; set; }
    [Required, StringLength(50)]
    public string FirstName { get; set; } = string.Empty;
    [Required, StringLength(50)]
    public string LastName { get; set; } = string.Empty;
    [Required, StringLength(20)]
    public string Phone { get; set; } = string.Empty;
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    [Required]
    public string Role { get; set; } = Roles.User;

    public string? ProfileImagePath { get; set; }
    
    public bool IsDeleted { get; set; } = false;
    public bool IsEmailVerified { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string? ResetToken { get; set; }
    public DateTime? ResetTokenExpiry { get; set; }

    public List<Order> Orders { get; set; } = new();
    public List<Review> Reviews { get; set; } = new();
}