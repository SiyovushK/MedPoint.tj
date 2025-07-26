using Domain.DTOs.ChatDTOs;

namespace Infrastructure.Interfaces;

public interface IChatService
{
    Task<ChatRoomDto> OpenRoomAsync(int userId, int doctorId);
    Task<List<ChatRoomDto>> ListDoctorRoomsAsync(int doctorId);
    Task<ChatMessageDto> SendMessageAsync(CreateChatMessageDto dto, int senderId);
    Task<List<ChatMessageDto>> GetMessagesAsync(int roomId, int take = 50);
}