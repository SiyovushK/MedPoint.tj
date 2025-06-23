namespace Domain.Filters;

public class UserFilter
{
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }

    public bool? IsDeleted { get; set; }
    public bool? IsEmailVerified { get; set; }
}