using Domain.DTOs.AdminDashboardDTOs;
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
}