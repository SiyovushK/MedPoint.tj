using Domain.DTOs.AdminDashboardDTOs;
using Domain.DTOs.DoctorDTOs;
using Domain.Enums;
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