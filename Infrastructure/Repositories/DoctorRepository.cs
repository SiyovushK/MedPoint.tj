using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class DoctorRepository(DataContext context) : IBaseRepository<Doctor, int>
{
    public async Task<int> AddAsync(Doctor entity)
    {
        await context.Doctors.AddAsync(entity);
        return await context.SaveChangesAsync();
    }

    public async Task<int> UpdateAsync(Doctor entity)
    {
        context.Doctors.Update(entity);
        return await context.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(Doctor entity)
    {
        context.Doctors.Remove(entity);
        return await context.SaveChangesAsync();
    }

    public IQueryable<Doctor> GetAll()
    {
        return context.Doctors.AsQueryable();
    }

    public async Task<Doctor?> GetByIdAsync(int id)
    {
        return await context.Doctors.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<List<Doctor>> GetByDoctorNameAsync(string doctorName)
    {
        var search = doctorName.ToLower();

        return await context.Doctors
            .Where(r =>
                r.FirstName.ToLower().Contains(search) ||
                r.LastName.ToLower().Contains(search) &&
                r.IsDeleted == false)
            .ToListAsync();
    }

    public async Task<Doctor?> GetByPhoneAsync(string phone)
    {
        return await context.Doctors.FirstOrDefaultAsync(u => u.Phone == phone);
    }

    public async Task<Doctor?> GetByEmailAsync(string email)
    {
        var emailLower = email.ToLower();
        return await context.Doctors.FirstOrDefaultAsync(u => u.Email.ToLower() == emailLower);
    }

    public async Task<int> MarkDoctorAsDeleted(int id)
    {
        var Doctor = await context.Doctors.FirstOrDefaultAsync(u => u.Id == id);
        Doctor!.IsDeleted = true;
        context.Doctors.Update(Doctor);
        return await context.SaveChangesAsync();
    }
    
    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync();
    }
}