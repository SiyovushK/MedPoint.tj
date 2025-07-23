using Domain.Constants;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository(DataContext context) : IBaseRepository<User, int>
{
    public async Task<int> AddAsync(User entity)
    {
        await context.Users.AddAsync(entity);
        return await context.SaveChangesAsync();
    }

    public async Task<int> UpdateAsync(User entity)
    {
        context.Users.Update(entity);
        return await context.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(User entity)
    {
        context.Users.Remove(entity);
        return await context.SaveChangesAsync();
    }

    public IQueryable<User> GetAll()
    {
        return context.Users
            .Where(u => u.Role == Roles.User)
            .AsQueryable();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<List<User>> GetByUserNameAsync(string userName)
    {
        return await context.Users
            .Where(r =>
                r.FirstName.ToLower().Contains(userName.ToLower()) ||
                r.LastName.ToLower().Contains(userName.ToLower()) &&
                r.IsDeleted == false)
            .ToListAsync();
    }

    public async Task<User?> GetByPhoneAsync(string phone)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Phone == phone);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var emailLower = email.ToLower();
        return await context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == emailLower);
    }

    public async Task<int> MarkUserAsDeleted(int id)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);
        user!.IsDeleted = true;
        context.Users.Update(user);
        return await context.SaveChangesAsync();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync();
    }

}  