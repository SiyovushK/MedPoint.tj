using System.Security.Claims;
using Domain.DTOs.AdminDashboardDTOs;
using Domain.DTOs.DoctorDTOs;
using Domain.Responses;

namespace Infrastructure.Interfaces;

public interface IAdminDashboardService
{
    Task<Response<CountStatisticsDTO>> GetCountStatistics();
    Task<Response<List<MonthlyCountStatistics>>> GetMonthlyCountStatistics();
    Task<Response<List<PopularDoctorDTO>>> GetPopularDoctors();
} 