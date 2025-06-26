namespace Domain.Filters;

public class ReviewFilter
{
    public int? DoctorId { get; set; }
    public int? UserId { get; set; }

    public int? RatingFrom { get; set; }
    public int? RatingTo { get; set; }

    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }

    public bool? IsHidden { get; set; }
}