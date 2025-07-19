using Domain.DTOs.DoctorDTOs;
using Domain.DTOs.OrderDTOs;
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
            .OrderByDescending(q => q.Date)
            .ThenByDescending(q => q.StartTime)
            .ToListAsync();
    }

    public async Task<List<Order>> GetByDoctorIdAsync(int doctorId)
    {
        return await context.Orders
            .Include(r => r.User)
            .Include(r => r.Doctor)
            .Where(r => r.DoctorId == doctorId)
            .OrderByDescending(q => q.Date)
            .ThenByDescending(q => q.StartTime)
            .ToListAsync();
    }

    public async Task<List<Order>> GetByDoctorScheduleAsync(int doctorId, DateOnly date)
    {
        return await context.Orders
            .Include(r => r.User)
            .Include(r => r.Doctor)
            .Where(r => r.DoctorId == doctorId && r.Date == date)
            .OrderByDescending(q => q.Date)
            .ThenByDescending(q => q.StartTime)
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

    public async Task<List<OrderStatisticsDTO>> GetMonthlyStatisticsAsync(int doctorId)
    {
        var dates = await context.Orders
            .Where(o => o.DoctorId == doctorId)
            .Select(o => o.CreatedAt)
            .ToListAsync();
        if (!dates.Any())
            return new List<OrderStatisticsDTO>();

        var minDate = new DateTime(dates.Min(d => d.Year), dates.Min(d => d.Month), 1);
        var maxDate = new DateTime(dates.Max(d => d.Year), dates.Max(d => d.Month), 1);

        var months = new List<string>();
        for (var dt = minDate; dt <= maxDate; dt = dt.AddMonths(1))
            months.Add($"{dt.Year}-{dt.Month:D2}");

        var grouped = await context.Orders
            .Where(o => o.DoctorId == doctorId)
            .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
            .Select(g => new
            {
                Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                Finished         = g.Count(o => o.OrderStatus == OrderStatus.Finished),
                NotAccepted      = g.Count(o => o.OrderStatus == OrderStatus.NotAccepted),
                CancelledByUser  = g.Count(o => o.OrderStatus == OrderStatus.CancelledByUser),
                CancelledByDoctor= g.Count(o => o.OrderStatus == OrderStatus.CancelledByDoctor)
            })
            .ToListAsync();

        var result = months
            .OrderBy(m => m)
            .Select(m => 
            {
                var g = grouped.FirstOrDefault(x => x.Month == m);
                return new OrderStatisticsDTO
                {
                    Month            = m,
                    Finished         = g?.Finished ?? 0,
                    NotAccepted      = g?.NotAccepted ?? 0,
                    CancelledByUser  = g?.CancelledByUser ?? 0,
                    CancelledByDoctor= g?.CancelledByDoctor ?? 0
                };
            })
            .ToList();

        return result;
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