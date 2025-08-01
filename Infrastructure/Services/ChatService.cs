using System.Net;
using Domain.Constants;
using Domain.DTOs.ChatDTOs;
using Domain.Entities;
using Domain.Responses;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class ChatService(DataContext _context) : IChatService
{
    public async Task<Response<int>> SendMessageAsync(int userId, int doctorId, string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return new Response<int>(HttpStatusCode.BadRequest, "Message text is required");

        var user = await _context.Users.FindAsync(userId);
        if (user == null || user.IsDeleted)
            return new Response<int>(HttpStatusCode.NotFound, "User not found");

        var doctor = await _context.Doctors.FindAsync(doctorId);
        if (doctor == null || !doctor.IsActive || doctor.IsDeleted)
            return new Response<int>(HttpStatusCode.NotFound, "Doctor not found");

        var chat = await _context.Chats
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.UserId == userId && c.DoctorId == doctorId);

        if (chat == null)
        {
            chat = new Chat
            {
                UserId = userId,
                DoctorId = doctorId
            };
            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();
        }

        var message = new Message
        {
            ChatId = chat.Id,
            SenderId = userId,
            Text = text,
            SentAt = DateTime.UtcNow
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        return new Response<int>(chat.Id);
    }

    public async Task<Response<List<MessageDto>>> GetChatMessagesAsync(int chatId, int requesterId)
    {
        var chat = await _context.Chats
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == chatId);

        if (chat == null)
            return new Response<List<MessageDto>>(HttpStatusCode.NotFound, "Chat not found");

        if (chat.UserId != requesterId && chat.DoctorId != requesterId)
            return new Response<List<MessageDto>>(HttpStatusCode.Forbidden, "Access denied");

        var messages = chat.Messages
            .Select(m => new MessageDto
            {
                Id = m.Id,
                SenderId = m.SenderId,
                Text = m.Text,
                SentAt = m.SentAt,
                IsRead = m.IsRead
            })
            .ToList();

        return new Response<List<MessageDto>>(messages);
    }

    public async Task<Response<List<ChatDto>>> GetUserChatsAsync(int userId)
    {
        var chats = await _context.Chats
            .Include(c => c.Messages)
            .Where(c => c.UserId == userId)
            .Select(c => new ChatDto
            {
                Id = c.Id,
                UserId = c.UserId,
                DoctorId = c.DoctorId,
                LastMessage = c.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault() != null
                    ? c.Messages.OrderByDescending(m => m.SentAt).First().Text
                    : "",
                LastMessageSentAt = c.Messages.Any()
                    ? c.Messages.Max(m => m.SentAt)
                    : DateTime.MinValue,
                UnreadMessages = c.Messages.Count(m => m.SenderId != userId && !m.IsRead)
            })
            .ToListAsync();

        return new Response<List<ChatDto>>(chats);
    }

    public async Task<Response<ChatInfoDto>> GetChatInfoAsync(int chatId, int requesterId)
    {
        var chat = await _context.Chats
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == chatId);

        if (chat == null)
            return new Response<ChatInfoDto>(HttpStatusCode.NotFound, "Chat not found");

        if (chat.UserId != requesterId && chat.DoctorId != requesterId)
            return new Response<ChatInfoDto>(HttpStatusCode.Forbidden, "Access denied");

        var lastMessage = chat.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault();

        var dto = new ChatInfoDto
        {
            Id = chat.Id,
            UserId = chat.UserId,
            DoctorId = chat.DoctorId,
            LastMessage = lastMessage?.Text ?? "",
            LastMessageSentAt = lastMessage?.SentAt ?? DateTime.MinValue,
            UnreadMessages = chat.Messages.Count(m => m.SenderId != requesterId && !m.IsRead)
        };

        return new Response<ChatInfoDto>(dto);
    }

    public async Task<Response<bool>> MarkAsReadAsync(int messageId, int requesterId)
    {
        var message = await _context.Messages.FindAsync(messageId);
        if (message == null)
            return new Response<bool>(HttpStatusCode.NotFound, "Message not found");

        var chat = await _context.Chats.FindAsync(message.ChatId);
        if (chat == null || (chat.UserId != requesterId && chat.DoctorId != requesterId))
            return new Response<bool>(HttpStatusCode.Forbidden, "Access denied");

        message.IsRead = true;
        await _context.SaveChangesAsync();

        return new Response<bool>(true);
    }
}