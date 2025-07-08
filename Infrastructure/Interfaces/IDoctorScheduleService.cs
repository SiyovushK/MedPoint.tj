using Domain.DTOs.ScheduleDTOs;
using Domain.Entities;
using Domain.Filters;
using Domain.Responses;

namespace Infrastructure.Interfaces;

public interface IDoctorScheduleService
{
    Task<Response<GetDoctorScheduleDTO>> CreateAsync(CreateDoctorScheduleDTO dto);
    Task<Response<GetDoctorScheduleDTO>> UpdateAsync(int id, UpdateDoctorScheduleDTO dto);
    Task<Response<string>> DeleteAsync(int id);
    Task<Response<GetDoctorScheduleDTO>> GetByIdAsync(int id);
    Task<Response<List<GetDoctorScheduleDTO>>> GetByDoctorIdAsync(int doctorId);
    Task<Response<List<GetDoctorScheduleDTO>>> GetAllAsync(DoctorScheduleFilter filter);
}