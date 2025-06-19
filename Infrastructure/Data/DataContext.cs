using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(u => u.Phone)
                .HasMaxLength(20);

            entity.Property(u => u.Email)
                .IsRequired();

            entity.HasIndex(u => u.Email)
                .IsUnique();

            entity.HasIndex(u => u.Phone)
                .IsUnique();
        });
    }
}