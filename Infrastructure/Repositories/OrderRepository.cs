using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class OrderRepository(DataContext context) : IBaseRepository<Order, int>
{
    public async Task<int> AddAsync(Order entity)
    {
        await context.Orders.AddAsync(entity);
        return await context.SaveChangesAsync();
    }

    public async Task<int> UpdateAsync(Order entity)
    {
        context.Orders.Update(entity);
        return await context.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(Order entity)
    {
        context.Orders.Remove(entity);
        return await context.SaveChangesAsync();
    }

    public IQueryable<Order> GetAll()
    {
        return context.Orders
            .Include(o => o.Doctor)
            .Include(o => o.User)
            .AsQueryable();
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await context.Orders
            .Include(o => o.Doctor)
            .Include(o => o.User)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<List<Order>> GetByUserIdAsync(int userId)
    {
        return await context.Orders
            .Include(r => r.User)
            .Include(r => r.Doctor)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.StartTime)
            .ToListAsync();
    }

    public async Task<List<Order>> GetByDoctorIdAsync(int doctorId)
    {
        return await context.Orders
            .Include(r => r.User)
            .Include(r => r.Doctor)
            .Where(r => r.DoctorId == doctorId)
            .OrderByDescending(r => r.StartTime)
            .ToListAsync();
    }

    public async Task<bool> IsDoctorBusyAsync(int doctorId, DateOnly date, TimeOnly startTime, TimeOnly endTime)
    {
        return await context.Orders
        .Where(o => o.DoctorId == doctorId &&
                    o.Date == date &&
                    o.OrderStatus != OrderStatus.CancelledByDoctor &&
                    o.OrderStatus != OrderStatus.CancelledByUser &&
                    o.OrderStatus != OrderStatus.Finished &&
                    (
                        startTime < o.EndTime && endTime > o.StartTime
                    ))
        .AnyAsync();
    }

    // Hangfire methods
    public async Task<List<Order>> GetPendingOrdersAsync()
    {
        return await context.Orders
            .Where(o => o.OrderStatus == OrderStatus.Pending)
            .ToListAsync();
    }

    public async Task<List<Order>> GetFinishedEligibleOrdersAsync(DateTime utcNow)
    {
        var date = DateOnly.FromDateTime(utcNow);
        var time = TimeOnly.FromDateTime(utcNow);

        return await context.Orders
            .Where(o => o.OrderStatus == OrderStatus.Active &&
                    (o.Date < date || (o.Date == date && o.EndTime <= time)))
            .ToListAsync();
    }
    
    public async Task<List<Order>> GetOrdersForUpcomingHourAsync(DateTime utcNow)
    {
        var startTime = utcNow.AddHours(1);
        var endTime = utcNow.AddHours(2);
        
        var startDate = DateOnly.FromDateTime(startTime);
        var startTimeOnly = TimeOnly.FromDateTime(startTime);
        var endDate = DateOnly.FromDateTime(endTime);
        var endTimeOnly = TimeOnly.FromDateTime(endTime);

        return await context.Orders
            .Include(o => o.User)
            .Include(o => o.Doctor)
            .Where(o =>
                o.OrderStatus == OrderStatus.Active &&
                !o.ReminderSent &&
                (o.Date > startDate || (o.Date == startDate && o.StartTime >= startTimeOnly)) &&
                (o.Date < endDate || (o.Date == endDate && o.StartTime < endTimeOnly))
            )
            .ToListAsync();
    }
}