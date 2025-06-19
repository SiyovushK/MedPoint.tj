using Domain.Entities;
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
        // services.AddScoped<IUserService, UserService>();
        services.AddScoped<IBaseRepository<User, int>, UserRepository>();

        services.AddScoped(typeof(IAuthRepository<>), typeof(AuthRepository<>));
        services.AddScoped<IAuthService, AuthService>();
        
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
    }
}