using Domain.DTOs.AdminDashboardDTOs;
using Domain.DTOs.DoctorDTOs;
using Domain.Responses;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;

namespace Infrastructure.Services;

public class AdminDashboardService
    (AdminDashboardRepository adminDashboard) : IAdminDashboardService
{
    public async Task<Response<CountStatisticsDTO>> GetCountStatistics()
    {
        var getStatistics = await adminDashboard.GetCountStatistics();
        return new Response<CountStatisticsDTO>(getStatistics);
    }

    public async Task<Response<List<UsersDoctorsStats>>> GetMonthlyCountStatisticsUsers()
    {
        var getStatistics = await adminDashboard.GetMonthlyStatisticsUsersAsync();
        return new Response<List<UsersDoctorsStats>>(getStatistics);
    }

    public async Task<Response<List<OrdersReviewsCount>>> GetMonthlyCountStatisticsOrders()
    {
        var getStatistics = await adminDashboard.GetMonthlyStatisticsOrdersAsync();
        return new Response<List<OrdersReviewsCount>>(getStatistics);
    }

    public async Task<Response<List<PopularDoctorDTO>>> GetPopularDoctors()
    {
        var getPopularDoctors = await adminDashboard.GetPopularDoctors();
        return new Response<List<PopularDoctorDTO>>(getPopularDoctors);
    }

    public async Task<Response<List<MonthComparisonDTO>>> GetChangeByMonthInfoAsync()
    {
        var usersStats = await adminDashboard.GetUsersChangeByMonth();
        var doctorsStats = await adminDashboard.GetDoctorsChangeByMonth();
        var ordersStats = await adminDashboard.GetOrdersChangeByMonth();
        var reviewsStats = await adminDashboard.GetReviewsChangeByMonth();

        var stats = new List<MonthComparisonDTO>
        {
            usersStats,
            doctorsStats,
            ordersStats,
            reviewsStats
        };

        return new Response<List<MonthComparisonDTO>>(stats);
    }
} 