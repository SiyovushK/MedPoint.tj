using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Data.Seeder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Extensions;

public static class SeederConfiguration
{
    public static async Task ApplyMigrationsAndSeedAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var provider = scope.ServiceProvider;

        var loggerFactory = provider.GetRequiredService<ILoggerFactory>();

        try
        {
            var context = provider.GetRequiredService<DataContext>();
            var passwordHasher = provider.GetRequiredService<IPasswordHasher<User>>();

            await context.Database.MigrateAsync();
            await DefaultUser.SeedAsync(context, passwordHasher);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}