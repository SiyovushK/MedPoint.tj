using Domain.Enums;

namespace Domain.Entities;

public class Doctor
{
    public int Id { get; set; }  
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty; 
    public string Phone { get; set; } = string.Empty; 
    public string Email { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public DoctorSpecialization Specialization { get; set; }

    public List<Review> Reviews { get; set; } = new();
    public List<Order> Orders { get; set; } = new();
}