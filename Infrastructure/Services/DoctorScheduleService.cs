using System.Net;
using AutoMapper;
using Domain.DTOs.ScheduleDTOs;
using Domain.Entities;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class DoctorScheduleService(
        DoctorScheduleRepository repository,
        DoctorRepository doctorRepository,
        IMapper mapper) : IDoctorScheduleService
{
    public async Task<Response<GetDoctorScheduleDTO>> CreateAsync(CreateDoctorScheduleDTO dto)
    {
        var doctor = await doctorRepository.GetByIdAsync(dto.DoctorId);
        if (doctor == null || doctor.IsDeleted)
            return new Response<GetDoctorScheduleDTO>(HttpStatusCode.NotFound, $"Doctor with ID {dto.DoctorId} not found.");

        var exists = await repository.GetAll().AnyAsync(x => x.DoctorId == dto.DoctorId && x.DayOfWeek == dto.DayOfWeek);
        if (exists)
            return new Response<GetDoctorScheduleDTO>(HttpStatusCode.Conflict, $"Schedule for Doctor ID {dto.DoctorId} on {dto.DayOfWeek} already exists.");

        if (dto.IsDayOff)
        {
            dto.WorkStart = null;
            dto.WorkEnd = null;
            dto.LunchStart = null;
            dto.LunchEnd = null;
        }
        else if (!dto.IsDayOff)
        {
            if (dto.WorkEnd <= dto.WorkStart)
                return new Response<GetDoctorScheduleDTO>(HttpStatusCode.BadRequest, "WorkEnd must be after WorkStart.");

            if (dto.LunchStart.HasValue && dto.LunchEnd.HasValue)
            {
                if (dto.LunchEnd <= dto.LunchStart)
                    return new Response<GetDoctorScheduleDTO>(HttpStatusCode.BadRequest, "LunchEnd must be after LunchStart.");

                if (dto.LunchStart <= dto.WorkStart || dto.LunchEnd >= dto.WorkEnd)
                    return new Response<GetDoctorScheduleDTO>(HttpStatusCode.BadRequest, "Lunch break must be within work hours.");
            }
            else if (dto.LunchStart.HasValue || dto.LunchEnd.HasValue)
            {
                return new Response<GetDoctorScheduleDTO>(HttpStatusCode.BadRequest, "Both LunchStart and LunchEnd must be provided.");
            }
        }

        var schedule = mapper.Map<DoctorSchedule>(dto);

        if (await repository.AddAsync(schedule) == 0)
            return new Response<GetDoctorScheduleDTO>(HttpStatusCode.InternalServerError, "Error adding schedule for doctor");

        var getDto = mapper.Map<GetDoctorScheduleDTO>(schedule);
        
        return new Response<GetDoctorScheduleDTO>(getDto);
    }

    public async Task<Response<GetDoctorScheduleDTO>> UpdateAsync(int id, UpdateDoctorScheduleDTO dto)
    {
        var schedule = await repository.GetByIdAsync(id);
        if (schedule == null)
            return new Response<GetDoctorScheduleDTO>(HttpStatusCode.NotFound, "Schedule not found.");

        var doctor = await doctorRepository.GetByIdAsync(dto.DoctorId);
        if (doctor == null || doctor.IsDeleted)
            return new Response<GetDoctorScheduleDTO>(HttpStatusCode.NotFound, $"Doctor with ID {dto.DoctorId} not found.");

        if (dto.DoctorId != schedule.DoctorId || dto.DayOfWeek != schedule.DayOfWeek)
        {
            var exists = await repository.GetAll()
                .AnyAsync(x => x.DoctorId == dto.DoctorId && x.DayOfWeek == dto.DayOfWeek && x.Id != id);
            if (exists)
                return new Response<GetDoctorScheduleDTO>(HttpStatusCode.Conflict, $"Schedule for Doctor ID {dto.DoctorId} on {dto.DayOfWeek} already exists.");
        }

        if (!dto.IsDayOff)
        {
            if (dto.WorkEnd <= dto.WorkStart)
                return new Response<GetDoctorScheduleDTO>(HttpStatusCode.BadRequest, "WorkEnd must be after WorkStart.");

            if (dto.LunchStart.HasValue && dto.LunchEnd.HasValue)
            {
                if (dto.LunchEnd <= dto.LunchStart)
                    return new Response<GetDoctorScheduleDTO>(HttpStatusCode.BadRequest, "LunchEnd must be after LunchStart.");

                if (dto.LunchStart <= dto.WorkStart || dto.LunchEnd >= dto.WorkEnd)
                    return new Response<GetDoctorScheduleDTO>(HttpStatusCode.BadRequest, "Lunch break must be within work hours.");
            }
            else if (dto.LunchStart.HasValue || dto.LunchEnd.HasValue)
            {
                return new Response<GetDoctorScheduleDTO>(HttpStatusCode.BadRequest, "Both LunchStart and LunchEnd must be provided.");
            }
        }
        else
        {
            if (dto.WorkStart != default || dto.WorkEnd != default || dto.LunchStart != null || dto.LunchEnd != null)
                return new Response<GetDoctorScheduleDTO>(HttpStatusCode.BadRequest, "Day off must not include work or lunch times.");
        }

        mapper.Map(dto, schedule);

        if (await repository.UpdateAsync(schedule) == 0)
            return new Response<GetDoctorScheduleDTO>(HttpStatusCode.InternalServerError, "Error updating schedule.");

        var getDto = mapper.Map<GetDoctorScheduleDTO>(schedule);

        return new Response<GetDoctorScheduleDTO>(getDto);
    }

    public async Task<Response<string>> DeleteAsync(int id)
    {
        var doctorSchedule = await repository.GetByIdAsync(id);
        if (doctorSchedule == null)
            return new Response<string>($"Schedule by id {id} is not found");

        if (await repository.DeleteAsync(doctorSchedule) == 0)
            return new Response<string>($"Something went wrong when deletion doctors schedule by id {id}");

        return new Response<string>($"Shedule by id {id} is deleted successfully");
    }

    public async Task<Response<GetDoctorScheduleDTO>> GetByIdAsync(int id)
    {
        var schedule = await repository.GetByIdAsync(id);
        if (schedule == null)
            return new Response<GetDoctorScheduleDTO>(HttpStatusCode.NotFound, "Schedule not found");

        var dto = mapper.Map<GetDoctorScheduleDTO>(schedule);
        
        return new Response<GetDoctorScheduleDTO>(dto);
    }

    public async Task<Response<List<GetDoctorScheduleDTO>>> GetByDoctorIdAsync(int doctorId)
    {
        var schedule = await repository.GetByDoctorIdAsync(doctorId);
        if (schedule == null)
            return new Response<List<GetDoctorScheduleDTO>>(HttpStatusCode.BadRequest, "Schedule for doctor is not found");

        var dto = mapper.Map<List<GetDoctorScheduleDTO>>(schedule);

        return new Response<List<GetDoctorScheduleDTO>>(dto);
    }

    public async Task<Response<List<GetDoctorScheduleDTO>>> GetAllAsync(DoctorScheduleFilter filter)
    {
        var query = repository.GetAll();

        if (!string.IsNullOrWhiteSpace(filter.DoctorName))
        {
            var search = filter.DoctorName.ToLower();
            query = query.Where(d =>
                d.Doctor.FirstName.ToLower().Contains(search) ||
                d.Doctor.LastName.ToLower().Contains(search));
        }

        if (filter.DoctorId != null)
            query = query.Where(u => u.DoctorId == filter.DoctorId);

        if (filter.DayOfWeek != null)
            query = query.Where(u => u.DayOfWeek == filter.DayOfWeek);

        if (filter.WorkStart.HasValue)
            query = query.Where(r => r.WorkStart >= filter.WorkStart.Value);

        if (filter.WorkEnd.HasValue)
            query = query.Where(r => r.WorkEnd <= filter.WorkEnd.Value);

        if (filter.IsDayOff.HasValue)
            query = query.Where(u => u.IsDayOff == filter.IsDayOff.Value);

        var schedules = await query
            .OrderBy(u => u.DoctorId)
            .ThenBy(u => u.DayOfWeek)
            .ToListAsync();

        var getSchedulesDto = mapper.Map<List<GetDoctorScheduleDTO>>(schedules);

        return new Response<List<GetDoctorScheduleDTO>>(getSchedulesDto);
    }
}