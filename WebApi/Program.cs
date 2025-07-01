using Hangfire;
using Infrastructure.AutoMapper;
using Infrastructure.Data;
using Infrastructure.DI;
using Infrastructure.Extensions;
using Infrastructure.Hangfire;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// builder.WebHost.UseUrls("http://147.45.146.15:5063");

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .SetIsOriginAllowed(_ => true)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var connection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAutoMapper(typeof(InfrastructureProfile));
builder.Services.AddInfrastructure();
builder.Services.AddSwaggerConfiguration();
builder.Services.AddAuthenticationConfiguration(builder.Configuration);
builder.Services.AddHangfireConfiguration(builder.Configuration);
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlite(connection));

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var jobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

    jobManager.AddOrUpdate<OrderProcessingJob>(
        "process-pending-orders",
        job => job.ProcessPendingOrdersAsync(),
        Cron.Hourly);

    jobManager.AddOrUpdate<OrderProcessingJob>(
        "process-finished-orders",
        job => job.FinishedOrdersAsync(),
        Cron.Hourly);

    jobManager.AddOrUpdate<OrderProcessingJob>(
        "send-appointment-reminders",
        job => job.SendAppointmentRemindersAsync(),
        Cron.Hourly);
}

await app.Services.ApplyMigrationsAndSeedAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHangfireDashboard();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();