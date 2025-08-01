using Domain.DTOs.ChatDTOs;
using Domain.Responses;

namespace Infrastructure.Interfaces;

public interface IChatService
{
    Task<Response<int>> SendMessageAsync(int userId, int doctorId, string text);
    Task<Response<List<MessageDto>>> GetChatMessagesAsync(int chatId, int requesterId);
    Task<Response<List<ChatDto>>> GetUserChatsAsync(int userId);
    Task<Response<ChatInfoDto>> GetChatInfoAsync(int chatId, int requesterId);
    Task<Response<bool>> MarkAsReadAsync(int messageId, int requesterId);
}