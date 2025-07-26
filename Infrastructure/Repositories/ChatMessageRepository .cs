using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ChatMessageRepository : IChatMessageRepository
{
    private readonly DataContext _db;
    public ChatMessageRepository(DataContext db) => _db = db;

    public async Task SaveAsync(ChatMessage msg)
    {
        _db.ChatMessages.Add(msg);
        await _db.SaveChangesAsync();
    }

    public Task<List<ChatMessage>> GetByRoomAsync(int roomId, int take = 50)
        => _db.ChatMessages
              .Where(m => m.RoomId == roomId)
              .OrderByDescending(m => m.SentAt)
              .Take(take)
              .OrderBy(m => m.SentAt)
              .ToListAsync();
}