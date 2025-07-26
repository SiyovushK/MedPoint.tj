using Domain.DTOs.ChatDTOs;

namespace Infrastructure.Interfaces;

public interface IChatService
{
    Task<ChatMessageDto> SendMessageAsync(CreateChatMessageDto dto, string senderId, string senderName);
    Task<List<ChatMessageDto>> GetHistoryAsync(string roomId, int take = 50);
}