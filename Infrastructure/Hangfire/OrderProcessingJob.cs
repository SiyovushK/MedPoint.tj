using Domain.DTOs.EmailDTOs;
using Domain.DTOs.TimezoneDTO;
using Domain.Enums;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TimeZoneConverter;

namespace Infrastructure.Hangfire;

public class OrderProcessingJob
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TimezoneSettings _timezoneSettings;

    public OrderProcessingJob(IServiceScopeFactory scopeFactory, IOptions<TimezoneSettings> timezoneOptions)
    {
        _scopeFactory = scopeFactory;
        _timezoneSettings = timezoneOptions.Value;
    }

    public async Task FinishedOrdersAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var orderRepository = scope.ServiceProvider.GetRequiredService<OrderRepository>();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        var appTimeZone = TZConvert.GetTimeZoneInfo(_timezoneSettings.AppTimeZone);

        var localNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, appTimeZone);

        var ordersToFinish = await orderRepository.GetFinishedEligibleOrdersAsync(localNow);

        foreach (var order in ordersToFinish)
        {
            order.OrderStatus = OrderStatus.Finished;
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task SendAppointmentRemindersAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var orderRepository = scope.ServiceProvider.GetRequiredService<OrderRepository>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var appTimeZone = TZConvert.GetTimeZoneInfo(_timezoneSettings.AppTimeZone);
            var localNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, appTimeZone);

            var upcomingOrders = await orderRepository.GetOrdersForUpcomingHourAsync(localNow);

            if (!upcomingOrders.Any())
            {
                await transaction.CommitAsync();
                return;
            }

            foreach (var order in upcomingOrders)
            {
                var emailDto = new EmailDTO()
                {
                    To = order.User.Email,
                    Subject = $"Reminder of doctor appointment",
                    Body = $"Dear {order.User.FirstName},\n\n"
                         + $"We want to remind you that you have and appointment to the doctor {order.Doctor.FirstName} {order.Doctor.LastName} "
                         + $"today at {order.StartTime:HH\\:mm}."
                };

                await emailService.SendEmailAsync(emailDto);
                order.ReminderSent = true;
            }

            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Hangfire Error] SendAppointmentRemindersAsync failed: {ex.Message}");
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task ProcessPendingOrdersAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var orderRepository = scope.ServiceProvider.GetRequiredService<OrderRepository>();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        var now = DateTime.UtcNow;

        var pendingOrders = await orderRepository.GetPendingOrdersAsync();

        foreach (var order in pendingOrders)
        {
            if ((now - order.CreatedAt).TotalHours >= 24)
            {
                order.OrderStatus = OrderStatus.NotAccepted;
            }
        }

        await dbContext.SaveChangesAsync();
    }
}