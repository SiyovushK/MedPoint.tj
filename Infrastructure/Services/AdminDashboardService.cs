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

    public async Task<Response<List<PopularDoctorDTO>>> GetPopularDoctors()
    {
        var getPopularDoctors = await adminDashboard.GetPopularDoctors();
        return new Response<List<PopularDoctorDTO>>(getPopularDoctors);
    }
} 