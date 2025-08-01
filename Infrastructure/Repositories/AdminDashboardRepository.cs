using Domain.DTOs.AdminDashboardDTOs;
using Domain.DTOs.DoctorDTOs;
using Domain.Responses;
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

    public async Task<List<UsersDoctorsStats>> GetMonthlyStatisticsUsersAsync()
    {
        var doctors = await context.Doctors.Select(d => d.CreatedAt).ToListAsync();
        var users = await context.Users.Select(u => u.CreatedAt).ToListAsync();

        var doctorGroups = doctors
            .GroupBy(d => d.ToString("yyyy-MM"))
            .Select(g => new { Month = g.Key, Count = g.Count() })
            .ToList();

        var userGroups = users
            .GroupBy(u => u.ToString("yyyy-MM"))
            .Select(g => new { Month = g.Key, Count = g.Count() })
            .ToList();

        var allMonths = doctorGroups.Select(x => x.Month)
            .Union(userGroups.Select(x => x.Month))
            .Distinct()
            .Where(month => month != "0001-01")
            .OrderBy(x => x)
            .ToList();

        var stats = new List<UsersDoctorsStats>();

        foreach (var month in allMonths)
        {
            stats.Add(new UsersDoctorsStats
            {
                Month = month,
                DoctorsCount = doctorGroups.FirstOrDefault(x => x.Month == month)?.Count ?? 0,
                UsersCount = userGroups.FirstOrDefault(x => x.Month == month)?.Count ?? 0,
            });
        }

        return stats;
    }

    public async Task<List<OrdersReviewsCount>> GetMonthlyStatisticsOrdersAsync()
    {
        var reviews = await context.Reviews.Select(r => r.CreatedAt).ToListAsync();
        var orders = await context.Orders.Select(o => o.Date).ToListAsync();

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

        var stats = new List<OrdersReviewsCount>();

        foreach (var month in allMonths)
        {
            stats.Add(new OrdersReviewsCount
            {
                Month = month,
                ReviewsCount = reviewGroups.FirstOrDefault(x => x.Month == month)?.Count ?? 0,
                OrdersCount = orderGroups.FirstOrDefault(x => x.Month == month)?.Count ?? 0
            });
        }

        return stats;
    }

    public async Task<List<PopularDoctorDTO>> GetPopularDoctors()
    {
        var popularDoctors = await context.Orders
            .Where(o => o.DoctorId != null)
            .GroupBy(o => new { o.DoctorId, o.Doctor.FirstName, o.Doctor.LastName })
            .Select(g => new PopularDoctorDTO
            {
                DoctorId = g.Key.DoctorId.Value,
                DoctorName = g.Key.FirstName + " " + g.Key.LastName,
                OrderCount = g.Count()
            })
            .OrderByDescending(x => x.OrderCount)
            .ToListAsync();

        return popularDoctors;
    }

    public async Task<MonthComparisonDTO> GetUsersChangeByMonth()
    {
        int previousMonth = await context.Users
            .Where(u => u.CreatedAt >= DateTime.UtcNow.AddMonths(-2) &&
                        u.CreatedAt <= DateTime.UtcNow.AddMonths(-1))
            .CountAsync();

        int currentMonth = await context.Users
            .Where(u => u.CreatedAt >= DateTime.UtcNow.AddMonths(-1) &&
                        u.CreatedAt <= DateTime.UtcNow)
            .CountAsync();

        double percentageChange;

        if (previousMonth == 0)
        {
            if (currentMonth == 0)
            {
                percentageChange = 0.0;
            }
            else
            {
                percentageChange = 100.0;
            }
        }
        else
        {
            percentageChange = ((double)(currentMonth - previousMonth) / previousMonth) * 100.0;
        }

        var stats = new MonthComparisonDTO
        {
            Category = "Users",
            Current = currentMonth,
            Previous = previousMonth,
            PercenteDifference = percentageChange.ToString("F1")
        };

        return stats;
    }

    public async Task<MonthComparisonDTO> GetDoctorsChangeByMonth()
    {
        int previousMonth = await context.Doctors
            .Where(u => u.CreatedAt >= DateTime.UtcNow.AddMonths(-2) &&
                        u.CreatedAt <= DateTime.UtcNow.AddMonths(-1))
            .CountAsync();

        int currentMonth = await context.Doctors
            .Where(u => u.CreatedAt >= DateTime.UtcNow.AddMonths(-1) &&
                        u.CreatedAt <= DateTime.UtcNow)
            .CountAsync();

        double percentageChange;

        if (previousMonth == 0)
        {
            if (currentMonth == 0)
            {
                percentageChange = 0.0;
            }
            else
            {
                percentageChange = 100.0;
            }
        }
        else
        {
            percentageChange = ((double)(currentMonth - previousMonth) / previousMonth) * 100.0;
        }

        var stats = new MonthComparisonDTO
        {
            Category = "Doctors",
            Current = currentMonth,
            Previous = previousMonth,
            PercenteDifference = percentageChange.ToString("F1")
        };

        return stats;
    }

    public async Task<MonthComparisonDTO> GetOrdersChangeByMonth()
    { 
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var period2MonthsAgo = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-2));
        var period1MonthAgo = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1));

        int previousMonthOrders = await context.Orders
            .Where(o => o.Date >= period2MonthsAgo &&
                        o.Date <= period1MonthAgo)
            .CountAsync();

        int currentMonthOrders = await context.Orders
            .Where(o => o.Date >= period1MonthAgo &&
                        o.Date <= today)
            .CountAsync();

        double percentageChange;

        if (previousMonthOrders == 0)
        {
            if (currentMonthOrders == 0)
            {
                percentageChange = 0.0;
            }
            else
            {
                percentageChange = 100.0;
            }
        }
        else
        {
            percentageChange = ((double)(currentMonthOrders - previousMonthOrders) / previousMonthOrders) * 100.0;
        }

        var stats = new MonthComparisonDTO
        {
            Category = "Orders",
            Current = currentMonthOrders,
            Previous = previousMonthOrders,
            PercenteDifference = percentageChange.ToString("F1")
        };

        return stats;
    }

    public async Task<MonthComparisonDTO> GetReviewsChangeByMonth()
    {
        int previousMonth = await context.Reviews
            .Where(u => u.CreatedAt >= DateTime.UtcNow.AddMonths(-2) &&
                        u.CreatedAt <= DateTime.UtcNow.AddMonths(-1))
            .CountAsync();

        int currentMonth = await context.Reviews
            .Where(u => u.CreatedAt >= DateTime.UtcNow.AddMonths(-1) &&
                        u.CreatedAt <= DateTime.UtcNow)
            .CountAsync();

        double percentageChange;

        if (previousMonth == 0)
        {
            if (currentMonth == 0)
            {
                percentageChange = 0.0;
            }
            else
            {
                percentageChange = 100.0;
            }
        }
        else
        {
            percentageChange = ((double)(currentMonth - previousMonth) / previousMonth) * 100.0;
        }

        var stats = new MonthComparisonDTO
        {
            Category = "Reviews",
            Current = currentMonth,
            Previous = previousMonth,
            PercenteDifference = percentageChange.ToString("F1")
        };

        return stats;
    }
}