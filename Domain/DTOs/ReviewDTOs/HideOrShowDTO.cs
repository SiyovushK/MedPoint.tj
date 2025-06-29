using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs.ReviewDTOs;

public class HideOrShowDTO
{
    [Required]
    public int ReviewId { get; set; }
    [Required]
    public bool IsHidden { get; set; }
}