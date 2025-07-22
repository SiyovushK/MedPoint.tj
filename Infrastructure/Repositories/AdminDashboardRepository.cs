using Domain.DTOs.AdminDashboardDTOs;
using Domain.DTOs.DoctorDTOs;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AdminDashboardRepository(DataContext context)
{
    public async Task<CountStatisticsDTO> GetCountStatistics()
    {
        var usersCount = await context.Users.CountAsync();
        var doctorsCount = await context.Doctors.CountAsync();
        var reviewsCount = await context.Reviews.CountAsync();
        var ordersCount = await context.Orders.CountAsync();

        return new CountStatisticsDTO
        {
            UsersCount = usersCount,
            DoctorsCount = doctorsCount,
            ReviewsCount = reviewsCount,
            OrdersCount = ordersCount
        };
    }

    public async Task<List<MonthlyCountStatistics>> GetMonthlyStatisticsAsync()
    {
        var doctors = await context.Doctors.Select(d => d.CreatedAt).ToListAsync();
        var users = await context.Users.Select(u => u.CreatedAt).ToListAsync();
        var reviews = await context.Reviews.Select(r => r.CreatedAt).ToListAsync();
        var orders = await context.Orders.Select(o => o.CreatedAt).ToListAsync();

        var doctorGroups = doctors
            .GroupBy(d => d.ToString("yyyy-MM"))
            .Select(g => new { Month = g.Key, Count = g.Count() })
            .ToList();

        var userGroups = users
            .GroupBy(u => u.ToString("yyyy-MM"))
            .Select(g => new { Month = g.Key, Count = g.Count() })
            .ToList();

        var reviewGroups = reviews
            .GroupBy(r => r.ToString("yyyy-MM"))
            .Select(g => new { Month = g.Key, Count = g.Count() })
            .ToList();

        var orderGroups = orders
            .GroupBy(o => o.ToString("yyyy-MM"))
            .Select(g => new { Month = g.Key, Count = g.Count() })
            .ToList();

        var allMonths = doctorGroups.Select(x => x.Month)
            .Union(userGroups.Select(x => x.Month))
            .Union(reviewGroups.Select(x => x.Month))
            .Union(orderGroups.Select(x => x.Month))
            .Distinct()
            .Where(month => month != "0001-01")
            .OrderBy(x => x)
            .ToList();

        var stats = new List<MonthlyCountStatistics>();

        foreach (var month in allMonths)
        {
            stats.Add(new MonthlyCountStatistics
            {
                Month = month,
                DoctorsCount = doctorGroups.FirstOrDefault(x => x.Month == month)?.Count ?? 0,
                UsersCount = userGroups.FirstOrDefault(x => x.Month == month)?.Count ?? 0,
                ReviewsCount = reviewGroups.FirstOrDefault(x => x.Month == month)?.Count ?? 0,
                OrdersCount = orderGroups.FirstOrDefault(x => x.Month == month)?.Count ?? 0
            });
        }

        return stats;
    }

    public async Task<List<PopularDoctorDTO>> GetPopularDoctors()
    {
        var popularDoctors = await context.Orders
            .GroupBy(o => new { o.DoctorId, o.Doctor.FirstName, o.Doctor.LastName })
            .Select(g => new PopularDoctorDTO
            {
                DoctorId = g.Key.DoctorId,
                DoctorName = g.Key.FirstName + " " + g.Key.LastName,
                OrderCount = g.Count()
            })
            .OrderByDescending(x => x.OrderCount)
            .ToListAsync();

        return popularDoctors;
    }
}