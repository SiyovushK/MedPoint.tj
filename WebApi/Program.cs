using Domain.DTOs.TimezoneDTO;
using Hangfire;
using Infrastructure.AutoMapper;
using Infrastructure.Data;
using Infrastructure.DI;
using Infrastructure.Extensions;
using Infrastructure.Hangfire;
using Infrastructure.Services.HelperServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WebApi.Chat;

var builder = WebApplication.CreateBuilder(args);

// builder.WebHost.UseUrls("http://147.45.146.15:5063");

builder.Services.AddSignalR();

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
builder.Services
  .AddControllers()
  .AddJsonOptions(opts =>
  {
    opts.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
    opts.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter());
  });

builder.Services.AddSwaggerGen(c =>
{
  c.MapType<DateOnly>(()   => new OpenApiSchema { Type = "string", Format = "date" });
  c.MapType<TimeOnly>(()   => new OpenApiSchema { Type = "string", Format = "time" });
  c.SchemaFilter<TimeOnlySchemaFilter>();
});
builder.Services.AddAuthenticationConfiguration(builder.Configuration);
builder.Services.AddHangfireConfiguration(builder.Configuration);
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlite(connection));

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization();

builder.Services.Configure<TimezoneSettings>(
    builder.Configuration.GetSection("TimezoneSettings"));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var jobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

    jobManager.AddOrUpdate<OrderProcessingJob>(
        "process-pending-orders",
        job => job.ProcessPendingOrdersAsync(),
        "*/30 * * * *");

    jobManager.AddOrUpdate<OrderProcessingJob>(
        "process-finished-orders",
        job => job.FinishedOrdersAsync(),
        "*/30 * * * *");

    jobManager.AddOrUpdate<OrderProcessingJob>(
        "send-appointment-reminders",
        job => job.SendAppointmentRemindersAsync(),
        "*/15 * * * *");
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

app.UseCors("AllowFrontend");
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat");
app.Run();