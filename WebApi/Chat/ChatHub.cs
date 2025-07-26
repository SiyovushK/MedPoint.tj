using Domain.DTOs.ChatDTOs;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace WebApi.Chat;

public class ChatHub : Hub
{
    private readonly IChatService _chatService;
    public ChatHub(IChatService chatService) => _chatService = chatService;

    // Создать/получить комнату
    public Task<ChatRoomDto> OpenRoom(int doctorId)
    {
        var userId = int.Parse(Context.UserIdentifier!);
        return _chatService.OpenRoomAsync(userId, doctorId);
    }

    // Войти в группу комнаты
    public Task JoinRoom(int roomId)
        => Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());

    // Отправить сообщение
    public async Task SendMessage(CreateChatMessageDto dto)
    {
        var senderId = int.Parse(Context.UserIdentifier!);
        var saved = await _chatService.SendMessageAsync(dto, senderId);
        await Clients.Group(dto.RoomId.ToString())
                     .SendAsync("ReceiveMessage", saved);
    }
}