using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IChatRoomRepository
{
    Task<ChatRoom> GetOrCreateAsync(int userId, int doctorId);
    Task<List<ChatRoom>> GetByDoctorAsync(int doctorId);
    Task<bool> UserExistsAsync(int userId);
    Task<bool> DoctorExistsAsync(int userId);
}