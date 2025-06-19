using Domain.Constants;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Seeder;

public static class DefaultUser
{
    public static async Task SeedAsync(DataContext context, IPasswordHasher<User> passwordHasher)
    {
        //admin
        var adminEmail = "kurbonovs397@gmail.com";
        var adminPhone = "018581212";

        var emailCheckAdmin = await context.Users.FirstOrDefaultAsync(c => c.Email == adminEmail);
        var phoneCheckAdmin = await context.Users.FirstOrDefaultAsync(c => c.Phone == adminPhone);
        
        if (phoneCheckAdmin != null || emailCheckAdmin != null)
        {
            var admin = new User
            {
                FirstName = "Admin",
                LastName = "User",
                Phone = adminPhone,
                Email = adminEmail,
                Role = Roles.Admin
            };

            admin.PasswordHash = passwordHasher.HashPassword(admin, "admin");
            await context.Users.AddAsync(admin);
        }   


        //master
        var masterEmail = "kurbanovs397@gmail.com";
        var masterPhone = "018581313";
        
        var emailCheckMaster = await context.Users.FirstOrDefaultAsync(c => c.Email == masterEmail);
        var phoneCheckMaster = await context.Users.FirstOrDefaultAsync(c => c.Phone == masterPhone);
        
        if (phoneCheckMaster != null || emailCheckMaster != null)
        {
            var master = new User
            {
                FirstName = "Master",
                LastName = "Test",
                Phone = masterPhone,
                Email = masterEmail,
                Role = Roles.Master
            };

            master.PasswordHash = passwordHasher.HashPassword(master, "master");
            await context.Users.AddAsync(master);
        }    


        await context.SaveChangesAsync();
    }
}