using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class DoctorScheduleRepository(DataContext context) : IBaseRepository<DoctorSchedule, int>
{
    public async Task<int> AddAsync(DoctorSchedule entity)
    {
        await context.DoctorSchedules.AddAsync(entity);
        return await context.SaveChangesAsync();
    }

    public async Task<int> UpdateAsync(DoctorSchedule entity)
    {
        context.DoctorSchedules.Update(entity);
        return await context.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(DoctorSchedule entity)
    {
        context.DoctorSchedules.Remove(entity);
        return await context.SaveChangesAsync();
    }

    public IQueryable<DoctorSchedule> GetAll()
    {
        return context.DoctorSchedules
            .Include(u => u.Doctor)
            .AsQueryable();
    }

    public async Task<DoctorSchedule?> GetByIdAsync(int id)
    {
        return await context.DoctorSchedules
            .Include(u => u.Doctor)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<List<DoctorSchedule>> GetByDoctorIdAsync(int id)
    {
        return await context.DoctorSchedules
            .Include(u => u.Doctor)
            .Where(u => u.DoctorId == id)
            .OrderBy(u => u.DayOfWeek)
            .ToListAsync();
    }
}