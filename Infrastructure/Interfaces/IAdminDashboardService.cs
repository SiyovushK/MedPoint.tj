using Domain.DTOs.AdminDashboardDTOs;
using Domain.DTOs.DoctorDTOs;
using Domain.Responses;

namespace Infrastructure.Interfaces;

public interface IAdminDashboardService
{
    Task<Response<string>> DeleteDoctors();
    Task<Response<CountStatisticsDTO>> GetCountStatistics();
    Task<Response<List<UsersDoctorsStats>>> GetMonthlyCountStatisticsUsers();
    Task<Response<List<OrdersReviewsCount>>> GetMonthlyCountStatisticsOrders();
    Task<Response<List<PopularDoctorDTO>>> GetPopularDoctors();
    Task<Response<List<MonthComparisonDTO>>> GetChangeByMonthInfoAsync();
} 