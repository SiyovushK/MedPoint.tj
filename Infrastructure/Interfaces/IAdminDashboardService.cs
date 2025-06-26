using Domain.DTOs.AdminDashboardDTOs;
using Domain.Responses;

namespace Infrastructure.Interfaces;

public interface IAdminDashboardService
{
    Task<Response<CountStatisticsDTO>> GetCountStatistics();
} 