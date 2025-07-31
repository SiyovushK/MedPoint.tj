using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly DataContext _context;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(DataContext context, ILogger<ChatHub> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SendMessage(string toUserIdStr, string message)
    {
        var fromUserIdStr = Context.UserIdentifier;
        
        if (!int.TryParse(fromUserIdStr, out var fromUserId) || !int.TryParse(toUserIdStr, out var toUserId))
        {
            await Clients.Caller.SendAsync("Error", "Invalid user ID format");
            return;
        }

        // Проверяем существование пользователей
        var toUserExists = await _context.Users.AnyAsync(u => u.Id == toUserId);
        var toDoctorExists = await _context.Doctors.AnyAsync(d => d.Id == toUserId);
        
        if (!toUserExists && !toDoctorExists)
        {
            await Clients.Caller.SendAsync("Error", "User not found");
            return;
        }

        var messageEntity = new Message
        {
            FromUserId = fromUserId,
            ToUserId = toUserId,
            Content = message,
            SentAt = DateTime.UtcNow,
            IsRead = false
        };

        _context.Messages.Add(messageEntity);
        await _context.SaveChangesAsync();

        // Отправляем сообщение получателю, используя СТРОКОВЫЙ ID
        await Clients.User(toUserIdStr).SendAsync("ReceiveMessage", new
        {
            Id = messageEntity.Id,
            FromUserId = messageEntity.FromUserId,
            ToUserId = messageEntity.ToUserId,
            Content = message,
            SentAt = messageEntity.SentAt,
            IsRead = false
        });

        // Отправляем подтверждение отправителю
        await Clients.Caller.SendAsync("MessageSent", new
        {
            Id = messageEntity.Id,
            FromUserId = messageEntity.FromUserId,
            ToUserId = messageEntity.ToUserId,
            Content = message,
            SentAt = messageEntity.SentAt,
            IsRead = false
        });
    }

    public async Task MarkAsRead(string chatPartnerIdStr)
    {
        var currentUserIdStr = Context.UserIdentifier;
        
        // Преобразуем строковые ID в int
        if (!int.TryParse(currentUserIdStr, out var currentUserId) || !int.TryParse(chatPartnerIdStr, out var chatPartnerId))
        {
            // Можно отправить ошибку или просто тихо выйти
            return; 
        }

        // Теперь используем int переменные в запросе
        var messages = await _context.Messages
            .Where(m => m.FromUserId == chatPartnerId && m.ToUserId == currentUserId && !m.IsRead)
            .ToListAsync();

        foreach (var message in messages)
        {
            message.IsRead = true;
        }

        if (messages.Any())
        {
            await _context.SaveChangesAsync();
        }
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
        {
            _logger.LogInformation($"User {userId} connected to chat");
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
        {
            _logger.LogInformation($"User {userId} disconnected from chat");
        }
        await base.OnDisconnectedAsync(exception);
    }
}