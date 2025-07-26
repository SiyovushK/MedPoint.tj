using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IChatMessageRepository
{
    Task SaveAsync(ChatMessage msg);
    Task<List<ChatMessage>> GetByRoomAsync(int roomId, int take = 50);
}