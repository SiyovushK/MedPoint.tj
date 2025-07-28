using Domain.DTOs.DoctorDTOs;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class DoctorRepository(DataContext context) : IBaseRepository<Doctor, int>
{
    public async Task<int> AddAsync(Doctor entity)
    {
        await context.Doctors.AddAsync(entity);
        return await context.SaveChangesAsync();
    }

    public async Task<int> UpdateAsync(Doctor entity)
    {
        context.Doctors.Update(entity);
        return await context.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(Doctor entity)
    {
        context.Doctors.Remove(entity);
        return await context.SaveChangesAsync();
    }

    public IQueryable<Doctor> GetAll()
    {
        return context.Doctors.AsQueryable();
    }

    public async Task<Doctor?> GetByIdAsync(int id)
    {
        return await context.Doctors.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<List<Doctor>> GetByDoctorNameAsync(string doctorName)
    {
        var search = doctorName.ToLower();

        return await context.Doctors
            .Where(r =>
                r.FirstName.ToLower().Contains(search) ||
                r.LastName.ToLower().Contains(search) &&
                r.IsDeleted == false)
            .ToListAsync();
    }

    public async Task<Doctor?> GetByPhoneAsync(string phone)
    {
        return await context.Doctors.FirstOrDefaultAsync(u => u.Phone == phone);
    }

    public async Task<Doctor?> GetByEmailAsync(string email)
    {
        var emailLower = email.ToLower();
        return await context.Doctors.FirstOrDefaultAsync(u => u.Email.ToLower() == emailLower);
    }

    public async Task<int> MarkDoctorAsDeleted(int id)
    {
        var Doctor = await context.Doctors.FirstOrDefaultAsync(u => u.Id == id);
        Doctor!.IsDeleted = true;
        context.Doctors.Update(Doctor);
        return await context.SaveChangesAsync();
    }

    public async Task<DoctorStatisticsDTO> GetDoctorStatistics(int doctorId)
    {
        var doctorName = await context.Doctors
            .Where(o => o.Id == doctorId)
            .Select(o => new DoctorStatisticsDTO { DoctorName = o.FirstName + " " + o.LastName })
            .FirstOrDefaultAsync();

        var orderCount = await context.Orders
            .Where(o => o.DoctorId == doctorId)
            .CountAsync();

        var reviewCount = await context.Reviews
            .Where(r => r.DoctorId == doctorId)
            .CountAsync();

        double avgRating = 0.0;
        if (reviewCount > 0)
        {
            avgRating = await context.Reviews
                .Where(r => r.DoctorId == doctorId)
                .AverageAsync(r => r.Rating);
        }

        var dto = new DoctorStatisticsDTO
        {
            DoctorId = doctorId,
            DoctorName = doctorName!.DoctorName,
            OrderCount = orderCount,
            ReviewCount = reviewCount,
            AverageRating = avgRating
        };

        return dto;
    }

    public async Task<List<DoctorMonthlyStatistics>> GetMonthlyStatisticsOrdersAsync(int doctorId)
    {
        var doctor = await context.Doctors.Where(d => d.Id == doctorId).FirstOrDefaultAsync();
        var reviews = await context.Reviews.Where(r => r.DoctorId == doctorId).Select(r => r.CreatedAt).ToListAsync();
        var orders = await context.Orders.Where(r => r.DoctorId == doctorId).Select(o => o.Date).ToListAsync();

        var reviewGroups = reviews
            .GroupBy(r => r.ToString("yyyy-MM"))
            .Select(g => new { Month = g.Key, Count = g.Count() })
            .ToList();

        var orderGroups = orders
            .GroupBy(o => o.ToString("yyyy-MM"))
            .Select(g => new { Month = g.Key, Count = g.Count() })
            .ToList();

        var allMonths = orderGroups.Select(x => x.Month)
            .Union(reviewGroups.Select(x => x.Month))
            .Distinct()
            .Where(month => month != "0001-01")
            .OrderBy(x => x)
            .ToList();

        var stats = new List<DoctorMonthlyStatistics>();

        foreach (var month in allMonths)
        {
            stats.Add(new DoctorMonthlyStatistics
            {
                Month = month,
                DoctorId = doctorId,
                DoctorName = $"{doctor!.FirstName} {doctor.LastName}",
                ReviewCount = reviewGroups.FirstOrDefault(x => x.Month == month)?.Count ?? 0,
                OrderCount = orderGroups.FirstOrDefault(x => x.Month == month)?.Count ?? 0
            });
        }

        return stats;
    }
    
    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync();
    }
}