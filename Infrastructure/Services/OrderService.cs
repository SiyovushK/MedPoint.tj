using System.Net;
using System.Security.Claims;
using AutoMapper;
using Domain.DTOs.EmailDTOs;
using Domain.DTOs.OrderDTOs;
using Domain.Entities;
using Domain.Enums;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class OrderService(
        IMapper mapper,
        OrderRepository orderRepository,
        DoctorRepository doctorRepository,
        DoctorScheduleRepository doctorScheduleRepository,
        UserRepository userRepository,
        DataContext dataContext,
        IEmailService emailService) : IOrderService
{
    public async Task<Response<GetOrderDTO>> CreateAsync(ClaimsPrincipal userClaims, CreateOrderDTO createOrder)
    {
        var userIdClaim = userClaims.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            return new Response<GetOrderDTO>(HttpStatusCode.Unauthorized, "User ID not found in token.");

        // 1) User
        var user = await userRepository.GetByIdAsync(userId);
        if (user == null || user.IsDeleted)
            return new Response<GetOrderDTO>(HttpStatusCode.NotFound, $"User with ID {userId} not found.");

        // 2) Doctor
        var doctor = await doctorRepository.GetByIdAsync(createOrder.DoctorId);
        if (doctor == null || doctor.IsDeleted)
            return new Response<GetOrderDTO>(HttpStatusCode.NotFound, $"Doctor with ID {createOrder.DoctorId} not found.");
        if (!doctor.IsActive)
            return new Response<GetOrderDTO>(HttpStatusCode.BadRequest, $"Doctor with ID {createOrder.DoctorId} is not available at the moment.");

        // 3) Date check
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var maxDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1));
        if (createOrder.Date < today)
            return new Response<GetOrderDTO>(HttpStatusCode.BadRequest, "Date of reservation can't be in the past.");
        if (createOrder.Date > maxDate)
            return new Response<GetOrderDTO>(HttpStatusCode.BadRequest, "Please select reservation date within a month.");

        var nowTime = TimeOnly.FromDateTime(DateTime.UtcNow);
        if (createOrder.Date == today && createOrder.StartTime < nowTime)
            return new Response<GetOrderDTO>(HttpStatusCode.BadRequest, "Start time can't be in the past.");

        // 5) Schedule of doctor
        var dayOfWeek = createOrder.Date.DayOfWeek;
        var schedule = await doctorScheduleRepository
            .GetAll()
            .FirstOrDefaultAsync(s => s.DoctorId == createOrder.DoctorId && s.DayOfWeek == dayOfWeek);

        if (schedule == null)
            return new Response<GetOrderDTO>(HttpStatusCode.BadRequest, $"Doctor's schedule is unavailable.");

        if (schedule.IsDayOff)
            return new Response<GetOrderDTO>(HttpStatusCode.BadRequest, $"Doctor is off on {dayOfWeek}.");

        // 6) Check order time
        var endTime = createOrder.StartTime.AddMinutes(30);
        if (createOrder.StartTime < schedule.WorkStart || endTime > schedule.WorkEnd)
            return new Response<GetOrderDTO>(HttpStatusCode.BadRequest,
                $"Please pick a time between {schedule.WorkStart:HH\\:mm} and {schedule.WorkEnd:HH\\:mm}.");

        if (schedule.LunchStart.HasValue && schedule.LunchEnd.HasValue)
        {
            if (createOrder.StartTime < schedule.LunchEnd.Value && endTime > schedule.LunchStart.Value)
                return new Response<GetOrderDTO>(HttpStatusCode.BadRequest,
                    $"The selected slot {createOrder.StartTime:HH\\:mm}-{endTime:HH\\:mm} falls into lunch break " +
                    $"({schedule.LunchStart:HH\\:mm}-{schedule.LunchEnd:HH\\:mm}).");
        }

        await using var tx = await dataContext.Database.BeginTransactionAsync();
        try
        {
            var busy = await orderRepository.IsDoctorBusyAsync(
                createOrder.DoctorId, createOrder.Date, createOrder.StartTime, endTime);
            if (busy)
                return new Response<GetOrderDTO>(HttpStatusCode.BadRequest, "Time slot already booked.");

            var order = mapper.Map<Order>(createOrder);
            order.UserId = userId;
            order.EndTime = endTime;

            if (await orderRepository.AddAsync(order) == 0)
                throw new Exception("DB insert returned 0");

            await tx.CommitAsync();

            var dto = mapper.Map<GetOrderDTO>(order);
            dto.UserName   = $"{user.FirstName} {user.LastName}";
            dto.DoctorName = $"{doctor.FirstName} {doctor.LastName}";

            var emailDto = new EmailDTO
            {
                To      = user.Email,
                Subject = "Reservation info",
                Body    = $"Hello {user.FirstName},\n\n" +
                        $"Your reservation with Dr. {dto.DoctorName} on {dto.Date:yyyy-MM-dd} " +
                        $"at {dto.StartTime:HH\\:mm} is created. You will be notified when it's confirmed.\n\n" +
                        "Best regards, MedPoint Team."
            };
            await emailService.SendEmailAsync(emailDto);

            return new Response<GetOrderDTO>(dto);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            Console.WriteLine(ex.Message);
            return new Response<GetOrderDTO>(HttpStatusCode.InternalServerError, $"Transaction failed: {ex.Message}");
        }
    }

    public async Task<Response<GetOrderDTO>> CreateByAdminAsync(CreateOrderByAdminDTO createOrder)
    {
        // 1) User
        var user = await userRepository.GetByIdAsync(createOrder.UserId);
        if (user == null || user.IsDeleted)
            return new Response<GetOrderDTO>(HttpStatusCode.NotFound, $"User with ID {createOrder.UserId} not found.");

        // 2) Doctor
        var doctor = await doctorRepository.GetByIdAsync(createOrder.DoctorId);
        if (doctor == null || doctor.IsDeleted)
            return new Response<GetOrderDTO>(HttpStatusCode.NotFound, $"Doctor with ID {createOrder.DoctorId} not found.");
        if (!doctor.IsActive)
            return new Response<GetOrderDTO>(HttpStatusCode.BadRequest, $"Doctor with ID {createOrder.DoctorId} is not available at the moment.");

        // 3) Date check
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var maxDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1));
        if (createOrder.Date < today)
            return new Response<GetOrderDTO>(HttpStatusCode.BadRequest, "Date of reservation can't be in the past.");
        if (createOrder.Date > maxDate)
            return new Response<GetOrderDTO>(HttpStatusCode.BadRequest, "Please select reservation date within a month.");

        var nowTime = TimeOnly.FromDateTime(DateTime.UtcNow);
        if (createOrder.Date == today && createOrder.StartTime < nowTime)
            return new Response<GetOrderDTO>(HttpStatusCode.BadRequest, "Start time can't be in the past.");

        // 5) Schedule of doctor
        var dayOfWeek = createOrder.Date.DayOfWeek;
        var schedule = await doctorScheduleRepository
            .GetAll()
            .FirstOrDefaultAsync(s => s.DoctorId == createOrder.DoctorId && s.DayOfWeek == dayOfWeek);

        if (schedule == null)
            return new Response<GetOrderDTO>(HttpStatusCode.BadRequest, $"Doctor's schedule is unavailable.");

        if (schedule.IsDayOff)
            return new Response<GetOrderDTO>(HttpStatusCode.BadRequest, $"Doctor is off on {dayOfWeek}.");

        // 6) Check order time
        var endTime = createOrder.StartTime.AddMinutes(30);
        if (createOrder.StartTime < schedule.WorkStart || endTime > schedule.WorkEnd)
            return new Response<GetOrderDTO>(HttpStatusCode.BadRequest,
                $"Please pick a time between {schedule.WorkStart:HH\\:mm} and {schedule.WorkEnd:HH\\:mm}.");

        if (schedule.LunchStart.HasValue && schedule.LunchEnd.HasValue)
        {
            if (createOrder.StartTime < schedule.LunchEnd.Value && endTime > schedule.LunchStart.Value)
                return new Response<GetOrderDTO>(HttpStatusCode.BadRequest,
                    $"The selected slot {createOrder.StartTime:HH\\:mm}-{endTime:HH\\:mm} falls into lunch break " +
                    $"({schedule.LunchStart:HH\\:mm}-{schedule.LunchEnd:HH\\:mm}).");
        }

        await using var tx = await dataContext.Database.BeginTransactionAsync();
        try
        {
            var busy = await orderRepository.IsDoctorBusyAsync(
                createOrder.DoctorId, createOrder.Date, createOrder.StartTime, endTime);
            if (busy)
                return new Response<GetOrderDTO>(HttpStatusCode.BadRequest, "Time slot already booked.");

            var order = mapper.Map<Order>(createOrder);
            order.EndTime = endTime;

            if (await orderRepository.AddAsync(order) == 0)
                throw new Exception("DB insert returned 0");

            await tx.CommitAsync();

            var dto = mapper.Map<GetOrderDTO>(order);
            dto.UserName = $"{user.FirstName} {user.LastName}";
            dto.DoctorName = $"{doctor.FirstName} {doctor.LastName}";

            var emailDto = new EmailDTO
            {
                To = user.Email,
                Subject = "Reservation info",
                Body = $"Hello {user.FirstName},\n\n" +
                        $"Your reservation with Dr. {dto.DoctorName} on {dto.Date:yyyy-MM-dd} " +
                        $"at {dto.StartTime:HH\\:mm} is created. You will be notified when it's confirmed.\n\n" +
                        "Best regards, MedPoint Team."
            };
            await emailService.SendEmailAsync(emailDto);

            return new Response<GetOrderDTO>(dto);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            Console.WriteLine(ex.Message);
            return new Response<GetOrderDTO>(HttpStatusCode.InternalServerError, $"Transaction failed: {ex.Message}");
        }
    }
    
    public async Task<Response<string>> DeleteAsync(int orderId)
    {
        var order = await orderRepository.GetByIdAsync(orderId);
        if (order == null)
            return new Response<string>(HttpStatusCode.BadRequest, $"Order with id {orderId} is not found");
        if (order.OrderStatus == OrderStatus.Pending || order.OrderStatus == OrderStatus.Active)
            return new Response<string>(HttpStatusCode.BadRequest, "Reservations with status Active or Pending can't be deleted");

        if (await orderRepository.DeleteAsync(order) == 0)
            return new Response<string>(HttpStatusCode.InternalServerError, "Error when deleting a reservation");

        return new Response<string>("Reservation deleted successfully");
    }


    public async Task<Response<GetOrderDTO>> GetByIdAsync(int orderId)
    {
        var order = await orderRepository.GetByIdAsync(orderId);
        if (order == null)
            return new Response<GetOrderDTO>(HttpStatusCode.BadRequest, $"Order with id {orderId} is not found");

        var getOrderDto = mapper.Map<GetOrderDTO>(order);

        return new Response<GetOrderDTO>(getOrderDto);
    }
    
    public async Task<Response<List<GetOrderDTO>>> GetUserOrdersAsync(int userId)
    {
        var orders = await orderRepository.GetByUserIdAsync(userId);
        if (orders == null)
            return new Response<List<GetOrderDTO>>(HttpStatusCode.NotFound, $"No orders found for user id {userId}.");

        var getOrder = mapper.Map<List<GetOrderDTO>>(orders);

        return new Response<List<GetOrderDTO>>(getOrder);
    }

    public async Task<Response<List<GetOrderDTO>>> GetUserOrdersFilteredAsync(UserOrderFilter filter)
    {
        if (filter.DateFrom.HasValue && filter.DateTo.HasValue && filter.DateFrom > filter.DateTo)
            return new Response<List<GetOrderDTO>>(HttpStatusCode.BadRequest, "'DateFrom' cannot be after 'DateTo'.");

        var user = await userRepository.GetByIdAsync(filter.UserId);
        if (user == null)
            return new Response<List<GetOrderDTO>>(HttpStatusCode.NotFound, $"User with ID {filter.UserId} not found.");

        var query = orderRepository
            .GetAll()
            .Where(r => r.UserId == filter.UserId);

        if (filter.DateFrom.HasValue)
            query = query.Where(r => r.Date >= filter.DateFrom.Value);

        if (filter.DateTo.HasValue)
            query = query.Where(r => r.Date <= filter.DateTo.Value);

        if (filter.OrderStatus.HasValue)
            query = query.Where(r => r.OrderStatus == filter.OrderStatus.Value);

        var orders = await query
            .OrderByDescending(q => q.StartTime)
            .ToListAsync();

        if (orders == null)
            return new Response<List<GetOrderDTO>>(HttpStatusCode.NotFound, $"No orders found for user id {filter.UserId}.");

        var getOrdersDto = mapper.Map<List<GetOrderDTO>>(orders);

        return new Response<List<GetOrderDTO>>(getOrdersDto);
    }

    public async Task<Response<List<GetOrderDTO>>> GetDoctorOrdersAsync(int doctorId)
    {
        var doctor = await doctorRepository.GetByIdAsync(doctorId);
        if (doctor == null)
            return new Response<List<GetOrderDTO>>(HttpStatusCode.NotFound, $"Doctor with ID {doctorId} not found.");

        var orders = await orderRepository.GetByDoctorIdAsync(doctorId);
        if (orders == null)
            return new Response<List<GetOrderDTO>>(HttpStatusCode.NotFound, $"No orders found for doctor id {doctorId}.");

        var getOrder = mapper.Map<List<GetOrderDTO>>(orders);

        return new Response<List<GetOrderDTO>>(getOrder);
    }

    public async Task<Response<List<GetOrderDTO>>> GetDoctorOrdersFilteredAsync(DoctorOrderFilter filter)
    {
        if (filter.DateFrom.HasValue && filter.DateTo.HasValue && filter.DateFrom > filter.DateTo)
            return new Response<List<GetOrderDTO>>(HttpStatusCode.BadRequest, "'DateFrom' cannot be after 'DateTo'.");

        var doctor = await doctorRepository.GetByIdAsync(filter.DoctorId);
        if (doctor == null)
            return new Response<List<GetOrderDTO>>(HttpStatusCode.NotFound, $"Doctor with ID {filter.DoctorId} not found.");

        var query = orderRepository
            .GetAll()
            .Where(r => r.DoctorId == filter.DoctorId);

        if (filter.DateFrom.HasValue)
            query = query.Where(r => r.Date >= filter.DateFrom.Value);

        if (filter.DateTo.HasValue)
            query = query.Where(r => r.Date <= filter.DateTo.Value);

        if (filter.OrderStatus.HasValue)
            query = query.Where(r => r.OrderStatus == filter.OrderStatus.Value);

        var orders = await query
            .OrderByDescending(q => q.StartTime)
            .ToListAsync();

        if (orders == null)
            return new Response<List<GetOrderDTO>>(HttpStatusCode.NotFound, $"No orders found for doctor id {filter.DoctorId}.");

        var getOrdersDto = mapper.Map<List<GetOrderDTO>>(orders);

        return new Response<List<GetOrderDTO>>(getOrdersDto);
    }

    public async Task<Response<List<OrderStatisticsDTO>>> GetDoctorsOrderStatisticsAsync(int doctorId)
    {
        var doctor = await doctorRepository.GetByIdAsync(doctorId);
        if (doctor == null)
            return new Response<List<OrderStatisticsDTO>>(HttpStatusCode.NotFound, $"Doctor with ID {doctorId} not found.");

        var orders = await orderRepository.GetMonthlyStatisticsAsync(doctorId);

        return new Response<List<OrderStatisticsDTO>>(orders);
    }

    public async Task<Response<List<GetOrderDTO>>> GetAllAsync(OrderFilter filter)
    {
        if (filter.DateFrom.HasValue && filter.DateTo.HasValue && filter.DateFrom > filter.DateTo)
            return new Response<List<GetOrderDTO>>(HttpStatusCode.BadRequest, "'DateFrom' cannot be after 'DateTo'.");

        if (filter.StartTimeFrom.HasValue && filter.StartTimeTo.HasValue && filter.StartTimeFrom > filter.StartTimeTo)
            return new Response<List<GetOrderDTO>>(HttpStatusCode.BadRequest, "'StartTimeFrom' cannot be after 'StartTimeTo'.");

        if (filter.CreatedFrom.HasValue && filter.CreatedTo.HasValue && filter.CreatedFrom > filter.CreatedTo)
            return new Response<List<GetOrderDTO>>(HttpStatusCode.BadRequest, "'CreatedFrom' cannot be after 'CreatedTo'.");

        var query = orderRepository.GetAll();

        if (filter.DoctorId.HasValue)
            query = query.Where(r => r.DoctorId == filter.DoctorId);

        if (!string.IsNullOrWhiteSpace(filter.DoctorName))
        {
            var search = filter.DoctorName.ToLower();
            query = query.Where(r =>
                r.Doctor.FirstName.ToLower().Contains(search) ||
                r.Doctor.LastName.ToLower().Contains(search));
        }

        if (filter.UserId.HasValue)
            query = query.Where(r => r.UserId == filter.UserId);

        if (!string.IsNullOrWhiteSpace(filter.UserName))
        {
            var search = filter.UserName.ToLower();
            query = query.Where(r =>
                r.User.FirstName.ToLower().Contains(search) ||
                r.User.LastName.ToLower().Contains(search));
        }

        if (filter.DateFrom.HasValue)
            query = query.Where(r => r.Date >= filter.DateFrom.Value);

        if (filter.DateTo.HasValue)
            query = query.Where(r => r.Date <= filter.DateTo.Value);

        if (filter.StartTimeFrom.HasValue)
            query = query.Where(r => r.StartTime >= filter.StartTimeFrom.Value);

        if (filter.StartTimeTo.HasValue)
            query = query.Where(r => r.StartTime <= filter.StartTimeTo.Value);

        if (filter.CreatedFrom.HasValue)
            query = query.Where(r => DateOnly.FromDateTime(r.CreatedAt) >= filter.CreatedFrom.Value);

        if (filter.CreatedTo.HasValue)
            query = query.Where(r => DateOnly.FromDateTime(r.CreatedAt) <= filter.CreatedTo.Value);

        if (filter.OrderStatus.HasValue)
            query = query.Where(r => r.OrderStatus == filter.OrderStatus.Value);

        var orders = await query.ToListAsync();

        var getOrdersDto = mapper.Map<List<GetOrderDTO>>(orders);

        return new Response<List<GetOrderDTO>>(getOrdersDto);
    }


    public async Task<Response<GetOrderDTO>> ConfirmOrderAsync(int orderId, ClaimsPrincipal doctorClaims)
    {
        var doctorIdClaim = doctorClaims.FindFirst(ClaimTypes.NameIdentifier);
        if (doctorIdClaim == null || !int.TryParse(doctorIdClaim.Value, out int doctorId))
            return new Response<GetOrderDTO>(HttpStatusCode.Unauthorized, "Doctor ID not found in token.");

        var order = await orderRepository.GetByIdAsync(orderId);
        if (order == null)
            return new Response<GetOrderDTO>(HttpStatusCode.NotFound, $"Order with id {orderId} is not found.");

        if (order.DoctorId != doctorId)
            return new Response<GetOrderDTO>(HttpStatusCode.Forbidden, "Order belongs to another doctor!");

        if (order.OrderStatus != OrderStatus.Pending)
            return new Response<GetOrderDTO>(HttpStatusCode.BadRequest, $"Only pending orders can be confirmed. Current status: {order.OrderStatus}");

        order.OrderStatus = OrderStatus.Active;

        if (await orderRepository.UpdateAsync(order) == 0)
            return new Response<GetOrderDTO>(HttpStatusCode.InternalServerError, "Failed to confirm the order.");

        var dto = mapper.Map<GetOrderDTO>(order);

        var emailDto = new EmailDTO
        {
            To = order.User.Email,
            Subject = "Reservation confirmation info",
            Body = $"Hello {order.User.FirstName},\n\nYour reservation made to doctor {dto.DoctorName} on date {dto.Date} {dto.StartTime:HH\\:mm} is confirmed!"
        };
        await emailService.SendEmailAsync(emailDto);

        return new Response<GetOrderDTO>(dto);
    }


    public async Task<Response<GetOrderDTO>> CancelOrderByDoctorAsync(CancelOrderDTO cancelOrder, ClaimsPrincipal doctorClaims)
    {
        var doctorIdClaim = doctorClaims.FindFirst(ClaimTypes.NameIdentifier);
        if (doctorIdClaim == null || !int.TryParse(doctorIdClaim.Value, out int doctorId))
            return new Response<GetOrderDTO>(HttpStatusCode.Unauthorized, "Doctor ID not found in token.");

        var order = await orderRepository.GetByIdAsync(cancelOrder.OrderId);
        if (order == null)
            return new Response<GetOrderDTO>(HttpStatusCode.NotFound, $"Order with id {cancelOrder.OrderId} is not found.");

        if (order.DoctorId != doctorId)
            return new Response<GetOrderDTO>(HttpStatusCode.Forbidden, "Order belongs to another doctor!");

        if (order.OrderStatus != OrderStatus.Pending && order.OrderStatus != OrderStatus.Active)
            return new Response<GetOrderDTO>(HttpStatusCode.BadRequest, $"Only Pending and Active orders can be cancelled. Current status: {order.OrderStatus}");

        order.OrderStatus = OrderStatus.CancelledByDoctor;
        order.CancellationReason = cancelOrder.Reason;

        if (await orderRepository.UpdateAsync(order) == 0)
            return new Response<GetOrderDTO>(HttpStatusCode.InternalServerError, "Failed to confirm the order.");

        var dto = mapper.Map<GetOrderDTO>(order);

        var emailDto = new EmailDTO
        {
            To = order.User.Email,
            Subject = "Reservation cancellation info",
            Body = $"Hello {order.User.FirstName},\n\nYour reservation made to doctor {dto.DoctorName} on date {dto.Date} {dto.StartTime.AddHours(5):HH\\:mm:HH\\:mm} has been cancelled by doctor!" +
                $"Cancellation reason: '{dto.CancellationReason}'"
        };
        await emailService.SendEmailAsync(emailDto);

        return new Response<GetOrderDTO>(dto);
    }

    public async Task<Response<GetOrderDTO>> CancelOrderByUserAsync(int orderId, ClaimsPrincipal userClaims)
    {
        var userIdClaim = userClaims.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            return new Response<GetOrderDTO>(HttpStatusCode.Unauthorized, "User ID not found in token.");

        var order = await orderRepository.GetByIdAsync(orderId);
        if (order == null)
            return new Response<GetOrderDTO>(HttpStatusCode.NotFound, $"Order with id {orderId} is not found.");

        if (order.UserId != userId)
            return new Response<GetOrderDTO>(HttpStatusCode.Forbidden, "Order belongs to another user!");

        if (order.OrderStatus != OrderStatus.Pending && order.OrderStatus != OrderStatus.Active)
            return new Response<GetOrderDTO>(HttpStatusCode.BadRequest, $"Only Pending and Active orders can be cancelled. Current status: {order.OrderStatus}");

        order.OrderStatus = OrderStatus.CancelledByUser;

        if (await orderRepository.UpdateAsync(order) == 0)
            return new Response<GetOrderDTO>(HttpStatusCode.InternalServerError, "Failed to confirm the order.");

        var dto = mapper.Map<GetOrderDTO>(order);

        var emailDto = new EmailDTO
        {
            To = order.User.Email,
            Subject = "Reservation cancellation info",
            Body = $"Hello {order.User.FirstName},\n\nYour reservation made to doctor {dto.DoctorName} on date {dto.Date} {dto.StartTime.AddHours(5):HH\\:mm} has been cancelled by you successfully!"
        };
        await emailService.SendEmailAsync(emailDto);

        return new Response<GetOrderDTO>(dto);
    }
}