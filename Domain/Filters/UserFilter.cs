namespace Domain.Filters;

public class UserFilter
{
    public int? Id { get; set; }
    public string? FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; } = string.Empty;
    public string? Phone { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;

    public bool? IsDeleted { get; set; }
    public bool? IsEmailVerified { get; set; }

    public int PageSize { get; set; }
    public int PageNumber { get; set; }
}