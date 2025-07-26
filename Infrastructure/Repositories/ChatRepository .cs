using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ChatRepository : IChatRepository
{
    private readonly DataContext _ctx;
    public ChatRepository(DataContext ctx) => _ctx = ctx;

    public async Task SaveMessageAsync(ChatMessage message)
    {
        _ctx.ChatMessages.Add(message);
        await _ctx.SaveChangesAsync();
    }

    public async Task<List<ChatMessage>> GetMessagesByRoomAsync(string roomId, int take = 50)
    {
        return await _ctx.ChatMessages
            .Where(m => m.RoomId == roomId)
            .OrderByDescending(m => m.SentAt)
            .Take(take)
            .OrderBy(m => m.SentAt)
            .ToListAsync();
    }
}