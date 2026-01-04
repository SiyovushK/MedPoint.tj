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

    public async Task UpdateAsync(User entity)
        => context.Users.Update(entity);

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
        => await context.Users.FirstOrDefaultAsync(u => u.Id == id);

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
        => await context.Users.FirstOrDefaultAsync(u => u.Phone == phone);

    public async Task<User?> GetByEmailAsync(string email)
        => await context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

    public async Task MarkUserAsDeleted(int id)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);
        await context.Users.ExecuteUpdateAsync(u => u.SetProperty(u => u.IsDeleted, true));
    }

}  