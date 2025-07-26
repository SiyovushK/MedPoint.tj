using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ChatRoomRepository(DataContext _db) : IChatRoomRepository
{
    public async Task<ChatRoom> GetOrCreateAsync(int userId, int doctorId)
    {
        var room = await _db.ChatRooms
            .Include(r => r.Messages)
            .FirstOrDefaultAsync(r => r.UserId == userId && r.DoctorId == doctorId);
        if (room != null) return room;

        room = new ChatRoom { UserId = userId, DoctorId = doctorId, CreatedAt = DateTime.UtcNow };
        _db.ChatRooms.Add(room);
        await _db.SaveChangesAsync();
        return room;
    }

    public Task<List<ChatRoom>> GetByDoctorAsync(int doctorId)
        => _db.ChatRooms
              .Where(r => r.DoctorId == doctorId)
              .OrderByDescending(r => r.CreatedAt)
              .ToListAsync();

    public Task<bool> UserExistsAsync(int userId)
        => _db.Users.AnyAsync(u => u.Id == userId);

    public Task<bool> DoctorExistsAsync(int doctorId)
        => _db.Doctors.AnyAsync(d => d.Id == doctorId);
}