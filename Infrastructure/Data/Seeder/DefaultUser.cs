using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Seeder;

public static class DefaultUser
{
    public static async Task SeedAsync(DataContext context, IPasswordHasher<User> passwordHasher, IPasswordHasher<Doctor> passwordHasherDoctor)
    {
        // Admin
        var adminEmail = "kurbonovs397@gmail.com";
        var adminPhone = "018581212";

        var emailCheckAdmin = await context.Users.FirstOrDefaultAsync(c => c.Email == adminEmail);
        var phoneCheckAdmin = await context.Users.FirstOrDefaultAsync(c => c.Phone == adminPhone);

        if (phoneCheckAdmin == null && emailCheckAdmin == null)
        {
            var admin = new User
            {
                FirstName = "Admin",
                LastName = "User",
                Phone = adminPhone,
                Email = adminEmail,
                Role = Roles.Admin,
                IsEmailVerified = true
            };

            admin.PasswordHash = passwordHasher.HashPassword(admin, "admin");
            await context.Users.AddAsync(admin);
        }

        // Doctor
        var doctorEmail = "kurbanovs397@gmail.com";
        var doctorPhone = "018581313";

        var emailCheckDoctor = await context.Doctors.FirstOrDefaultAsync(c => c.Email == doctorEmail);
        var phoneCheckDoctor = await context.Doctors.FirstOrDefaultAsync(c => c.Phone == doctorPhone);

        if (phoneCheckDoctor == null && emailCheckDoctor == null)
        {
            var doctor = new Doctor
            {
                FirstName = "Doctor",
                LastName = "Test",
                Phone = doctorPhone,
                Email = doctorEmail,
                Description = "Really good description",
                Specialization = new[] { DoctorSpecialization.MiddleDoctor }
            };

            doctor.PasswordHash = passwordHasherDoctor.HashPassword(doctor, "doctor");
            await context.Doctors.AddAsync(doctor);
        }

        await context.SaveChangesAsync();
    }
}