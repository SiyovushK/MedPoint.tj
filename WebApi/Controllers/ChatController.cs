using System.Net;
using System.Security.Claims;
using Domain.Constants;
using Domain.DTOs.ChatDTOs;
using Domain.Entities;
using Domain.Responses;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly DataContext _context;

    public ChatController(DataContext context)
    {
        _context = context;
    }

    [HttpGet("chats")]
    public async Task<ActionResult<IEnumerable<Chat>>> GetUserChats()
    {
        var currentUserIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // ИСПРАВЛЕНО: Преобразуем строковый ID в int в самом начале
        if (!int.TryParse(currentUserIdStr, out var currentUserId))
        {
            return Unauthorized();
        }

        // ИСПРАВЛЕНО: Все сравнения теперь происходят между int и int
        var groupedMessages = await _context.Messages
            .AsNoTracking()
            .Where(m => m.FromUserId == currentUserId || m.ToUserId == currentUserId)
            .GroupBy(m => m.FromUserId == currentUserId ? m.ToUserId : m.FromUserId)
            .Select(g => new
            {
                ChatPartnerId = g.Key,
                LastMessage = g.OrderByDescending(m => m.SentAt).First(),
                UnreadCount = g.Count(m => m.ToUserId == currentUserId && !m.IsRead)
            })
            .OrderByDescending(c => c.LastMessage.SentAt)
            .ToListAsync();

        if (!groupedMessages.Any())
        {
            return Ok(new List<Chat>());
        }

        // --- Оптимизация ---

        // ИСПРАВЛЕНО: partnerIds теперь коллекция типа int
        var partnerIds = groupedMessages.Select(g => g.ChatPartnerId).ToList();

        // ИСПРАВЛЕНО: Собираем ID типа int для запроса к БД
        var allUserIntIds = partnerIds.Concat(new[] { currentUserId }).Distinct().ToList();

        // 2. Одним запросом получаем имена всех нужных пользователей и докторов
        var userNames = await _context.Users
            // ИСПРАВЛЕНО: .Contains() теперь работает с int без .ToString()
            .Where(u => allUserIntIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id.ToString(), u => $"{u.FirstName} {u.LastName}");

        var doctorNames = await _context.Doctors
            .Where(d => allUserIntIds.Contains(d.Id))
            .ToDictionaryAsync(d => d.Id.ToString(), d => $"{d.FirstName} {d.LastName}");

        var allNames = userNames;
        foreach (var doc in doctorNames)
        {
            allNames[doc.Key] = doc.Value;
        }

        // --- Конец оптимизации ---

        var currentUserName = allNames.GetValueOrDefault(currentUserIdStr, "Unknown");

        var chats = groupedMessages.Select(group => new Chat
        {
            // ChatId может оставаться строковым представлением ID партнера для удобства маршрутизации на фронтенде
            ChatId = group.ChatPartnerId.ToString(),
            // Теперь User1Id и User2Id напрямую присваиваются как int
            User1Id = currentUserId, // Изменено
            User2Id = group.ChatPartnerId, // Изменено
            User1Name = currentUserName,
            User2Name = allNames.GetValueOrDefault(group.ChatPartnerId.ToString(), "Unknown"),
            LastMessageAt = group.LastMessage.SentAt,
            LastMessagePreview = group.LastMessage.Content,
            UnreadCount = group.UnreadCount
        }).ToList();

        return Ok(chats);
    }

    [HttpGet("messages/{userIdStr}")]
    public async Task<ActionResult<IEnumerable<Message>>> GetChatMessages(string userIdStr)
    {
        var currentUserIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // ИСПРАВЛЕНО: Преобразуем оба ID в int
        if (!int.TryParse(currentUserIdStr, out var currentUserId) || !int.TryParse(userIdStr, out var userId))
        {
            return BadRequest("Invalid user ID format.");
        }

        // ИСПРАВЛЕНО: Запрос теперь сравнивает int с int
        var messages = await _context.Messages
            .Where(m =>
                (m.FromUserId == currentUserId && m.ToUserId == userId) ||
                (m.FromUserId == userId && m.ToUserId == currentUserId))
            .OrderBy(m => m.SentAt)
            .ToListAsync();

        var unreadMessages = messages
            .Where(m => m.ToUserId == currentUserId && !m.IsRead)
            .ToList();

        foreach (var message in unreadMessages)
        {
            message.IsRead = true;
        }

        if (unreadMessages.Any())
        {
            await _context.SaveChangesAsync();
        }

        return Ok(messages);
    }

    [HttpGet("chat/{userIdStr}")]
    public async Task<ActionResult<Chat>> GetChatInfo(string userIdStr)
    {
        var currentUserIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // ИСПРАВЛЕНО: Преобразуем оба ID в int
        if (!int.TryParse(currentUserIdStr, out var currentUserId) || !int.TryParse(userIdStr, out var userId))
        {
            return BadRequest("Invalid user ID format.");
        }

        // ИСПРАВЛЕНО: Запрос теперь сравнивает int с int
        var chatData = await _context.Messages
            .AsNoTracking()
            .Where(m => (m.FromUserId == currentUserId && m.ToUserId == userId) ||
                        (m.FromUserId == userId && m.ToUserId == currentUserId))
            .GroupBy(m => 1)
            .Select(g => new
            {
                LastMessage = g.OrderByDescending(m => m.SentAt).FirstOrDefault(),
                UnreadCount = g.Count(m => m.ToUserId == currentUserId && !m.IsRead)
            })
            .FirstOrDefaultAsync();

        // Эта часть остается без изменений, т.к. она ищет имена по строковым ID
        var userIds = new[] { currentUserIdStr, userIdStr };
        var userNames = await _context.Users
            .Where(u => userIds.Contains(u.Id.ToString()))
            .ToDictionaryAsync(u => u.Id.ToString(), u => $"{u.FirstName} {u.LastName}");

        var doctorNames = await _context.Doctors
            .Where(d => userIds.Contains(d.Id.ToString()))
            .ToDictionaryAsync(d => d.Id.ToString(), d => $"{d.FirstName} {d.LastName}");

        var allNames = userNames;
        foreach (var doc in doctorNames) { allNames[doc.Key] = doc.Value; }

        var chat = new Chat
        {
            ChatId = userIdStr, // ChatId может оставаться строкой, если это просто идентификатор партнера для UI
            User1Id = currentUserId, // Изменено: теперь int
            User2Id = userId,        // Изменено: теперь int
            User1Name = allNames.GetValueOrDefault(currentUserIdStr, "Unknown"),
            User2Name = allNames.GetValueOrDefault(userIdStr, "Unknown"),
            LastMessageAt = chatData?.LastMessage?.SentAt ?? DateTime.MinValue,
            LastMessagePreview = chatData?.LastMessage?.Content ?? "",
            UnreadCount = chatData?.UnreadCount ?? 0
        };

        return Ok(chat);
    }
}