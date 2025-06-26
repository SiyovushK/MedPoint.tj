using Domain.DTOs.AdminDashboardDTOs;
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
} 