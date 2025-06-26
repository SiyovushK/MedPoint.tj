using Domain.Entities;
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
        return context.Orders.AsQueryable();
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await context.Orders.FirstOrDefaultAsync(u => u.Id == id);
    }
}