using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Review> Reviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(u => u.Phone)
                .HasMaxLength(20);

            entity.HasIndex(u => u.Email)
                .IsUnique();

            entity.HasIndex(u => u.Phone)
                .IsUnique();

            entity.HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(u => u.Reviews)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.Property(d => d.Phone)
                .HasMaxLength(20);

            entity.HasIndex(d => d.Email)
                .IsUnique();

            entity.HasIndex(d => d.Phone)
                .IsUnique();

            entity.Property(d => d.Description)
                .HasMaxLength(500);

            entity.HasMany(d => d.Orders)
                .WithOne(o => o.Doctor)
                .HasForeignKey(o => o.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(d => d.Reviews)
                .WithOne(r => r.Doctor)
                .HasForeignKey(r => r.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}