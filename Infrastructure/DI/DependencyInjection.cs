using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Hangfire;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DI;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<UserRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IBaseRepository<User, int>, UserRepository>();

        services.AddScoped<DoctorRepository>();
        services.AddScoped<IDoctorService, DoctorService>();
        services.AddScoped<IBaseRepository<Doctor, int>, DoctorRepository>();

        services.AddScoped<ReviewRepository>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IBaseRepository<Review, int>, ReviewRepository>();

        services.AddScoped<OrderRepository>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IBaseRepository<Order, int>, OrderRepository>();

        services.AddScoped<DoctorScheduleRepository>();
        services.AddScoped<IDoctorScheduleService, DoctorScheduleService>();
        services.AddScoped<IBaseRepository<DoctorSchedule, int>, DoctorScheduleRepository>();

        services.AddScoped<IChatService, ChatService>();

        services.AddScoped<IAdminDashboardService, AdminDashboardService>();
        services.AddScoped<AdminDashboardRepository>();

        services.AddScoped(typeof(IAuthRepository<>), typeof(AuthRepository<>));
        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<IPasswordHasher<Doctor>, PasswordHasher<Doctor>>();
        
        services.AddScoped<OrderProcessingJob>();
        services.AddScoped<DataContext>();
    }
}