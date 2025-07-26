using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IChatRepository
{
    Task SaveMessageAsync(ChatMessage message);
    Task<List<ChatMessage>> GetMessagesByRoomAsync(string roomId, int take = 50);
}