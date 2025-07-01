using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ReviewRepository(DataContext context) : IBaseRepository<Review, int>
{
    public async Task<int> AddAsync(Review entity)
    {
        await context.Reviews.AddAsync(entity);
        return await context.SaveChangesAsync();
    }

    public async Task<int> UpdateAsync(Review entity)
    {
        context.Reviews.Update(entity);
        return await context.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(Review entity)
    {
        context.Reviews.Remove(entity);
        return await context.SaveChangesAsync();
    }

    public IQueryable<Review> GetAll()
    {
        return context.Reviews
            .Include(r => r.User)
            .Include(r => r.Doctor)
            .OrderByDescending(r => r.CreatedAt)
            .AsQueryable();
    }

    public async Task<Review?> GetByIdAsync(int id)
    {
        return await context.Reviews
            .Include(r => r.User)
            .Include(r => r.Doctor)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<List<Review>> GetByUserIdAsync(int userId)
    {
        return await context.Reviews
            .Include(r => r.User)
            .Include(r => r.Doctor)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }   

    public async Task<List<Review>> GetByDoctorIdAsync(int doctorId)
    {
        return await context.Reviews
            .Include(r => r.User)
            .Include(r => r.Doctor)
            .Where(r => r.DoctorId == doctorId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }   
} 