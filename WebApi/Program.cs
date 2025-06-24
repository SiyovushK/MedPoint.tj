using Infrastructure.AutoMapper;
using Infrastructure.Data;
using Infrastructure.DI;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAutoMapper(typeof(InfrastructureProfile));
builder.Services.AddInfrastructure();
builder.Services.AddSwaggerConfiguration();
builder.Services.AddAuthenticationConfiguration(builder.Configuration);
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlite(connection));

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization();

var app = builder.Build();

await app.Services.ApplyMigrationsAndSeedAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();