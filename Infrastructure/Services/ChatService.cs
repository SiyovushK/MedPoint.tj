using Domain.DTOs.ChatDTOs;
using Domain.Entities;
using Infrastructure.Interfaces;

namespace Infrastructure.Services;

public class ChatService : IChatService
{
    private readonly IChatRepository _repo;
    public ChatService(IChatRepository repo) => _repo = repo;

    public async Task<ChatMessageDto> SendMessageAsync(CreateChatMessageDto dto, string senderId, string senderName)
    {
        var message = new ChatMessage
        {
            RoomId = dto.RoomId,
            SenderId = senderId,
            SenderName = senderName,
            Text = dto.Text,
            SentAt = DateTime.UtcNow
        };
        await _repo.SaveMessageAsync(message);
        return new ChatMessageDto
        {
            RoomId = message.RoomId,
            SenderId = message.SenderId,
            SenderName = message.SenderName,
            Text = message.Text,
            SentAt = message.SentAt
        };
    }

    public async Task<List<ChatMessageDto>> GetHistoryAsync(string roomId, int take = 50)
    {
        var msgs = await _repo.GetMessagesByRoomAsync(roomId, take);
        return msgs.Select(m => new ChatMessageDto
        {
            RoomId = m.RoomId,
            SenderId = m.SenderId,
            SenderName = m.SenderName,
            Text = m.Text,
            SentAt = m.SentAt
        }).ToList();
    }
}