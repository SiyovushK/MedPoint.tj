using Domain.DTOs.ChatDTOs;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace WebApi.Chat;

public class ChatHub : Hub
{
    private readonly IChatService _chatService;

    public ChatHub(IChatService chatService)
    {
        _chatService = chatService;
    }

    // Клиент вызывает этот метод, чтобы отправить сообщение
    public async Task SendMessage(CreateChatMessageDto dto)
    {
        // Получаем данные текущего пользователя
        var senderId = Context.UserIdentifier;       // при настройке JWT‑идентификации
        var senderName = Context.User?.Identity?.Name;

        // Сохраняем в БД и получаем DTO с SentAt
        var saved = await _chatService.SendMessageAsync(dto, senderId, senderName);

        // Рассылаем всем в группе (комнате)
        await Clients.Group(dto.RoomId)
            .SendAsync("ReceiveMessage", saved);
    }

    // Клиент присоединяется к комнате (doctor или support)
    public override async Task OnConnectedAsync()
    {
        var roomId = Context.GetHttpContext().Request.Query["roomId"];
        if (!string.IsNullOrEmpty(roomId))
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        await base.OnConnectedAsync();
    }
}