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

    public async Task<List<Order>> GetPendingOrdersAsync()
    {
        return await context.Orders
            .Where(o => o.OrderStatus == OrderStatus.Pending)
            .ToListAsync();
    }

    public async Task<List<Order>> GetFinishedEligibleOrdersAsync(DateTime utcNow)
    {
        return await context.Orders
            .Where(o => o.OrderStatus == OrderStatus.Active &&
                        o.Date.ToDateTime(o.EndTime) <= utcNow)
            .ToListAsync();
    }

    public async Task<List<Order>> GetOrdersForUpcomingHourAsync(DateTime utcNow)
    {
        var startTime = utcNow.AddHours(1);
        var endTime = utcNow.AddHours(2);

        return await context.Orders
            .Include(o => o.User)
            .Include(o => o.Doctor)
            .Where(o =>
                o.OrderStatus == OrderStatus.Active &&
                !o.ReminderSent &&
                o.Date.ToDateTime(o.StartTime) >= startTime &&
                o.Date.ToDateTime(o.StartTime) < endTime)
            .ToListAsync();
    }
}