using Domain.DTOs.ChatDTOs;
using Domain.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;

namespace Infrastructure.Services;

public class ChatService(
    IChatRoomRepository chatRoomRepository,
    IChatRoomRepository _rooms,
    IChatMessageRepository _msgs) : IChatService
{
    public async Task<ChatRoomDto> OpenRoomAsync(int userId, int doctorId)
    {
        if (!await chatRoomRepository.UserExistsAsync(userId))
            throw new KeyNotFoundException($"User with Id={userId} is not found");

        if (!await chatRoomRepository.DoctorExistsAsync(doctorId))
            throw new KeyNotFoundException($"Doctor with Id={doctorId} is not found");

        var room = await _rooms.GetOrCreateAsync(userId, doctorId);
        return new ChatRoomDto
        {
            Id = room.Id,
            UserId = room.UserId,
            DoctorId = room.DoctorId,
            CreatedAt = room.CreatedAt
        };
    }

    public async Task<List<ChatRoomDto>> ListDoctorRoomsAsync(int doctorId)
    {
        if (!await chatRoomRepository.DoctorExistsAsync(doctorId))
            throw new KeyNotFoundException($"Doctor with Id={doctorId} is not found");

        var list = await _rooms.GetByDoctorAsync(doctorId);
        return list.Select(r => new ChatRoomDto {
            Id = r.Id,
            UserId = r.UserId,
            DoctorId = r.DoctorId,
            CreatedAt = r.CreatedAt
        }).ToList();
    }

    public async Task<ChatMessageDto> SendMessageAsync(CreateChatMessageDto dto, int senderId)
    {
        var msg = new ChatMessage
        {
            RoomId = dto.RoomId,
            SenderId = senderId,
            Text = dto.Text,
            SentAt = DateTime.UtcNow
        };
        await _msgs.SaveAsync(msg);
        return new ChatMessageDto
        {
            RoomId = msg.RoomId,
            SenderId = msg.SenderId,
            Text = msg.Text,
            SentAt = msg.SentAt
        };
    }

    public async Task<List<ChatMessageDto>> GetMessagesAsync(int roomId, int take = 50)
    {
        var msgs = await _msgs.GetByRoomAsync(roomId, take);
        return msgs.Select(m => new ChatMessageDto {
            RoomId = m.RoomId,
            SenderId = m.SenderId,
            Text = m.Text,
            SentAt = m.SentAt
        }).ToList();
    }
}