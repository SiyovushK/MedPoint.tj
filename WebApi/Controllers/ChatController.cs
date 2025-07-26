using System.Net;
using System.Security.Claims;
using Domain.Constants;
using Domain.DTOs.ChatDTOs;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chat;
    public ChatController(IChatService chat) => _chat = chat;

    // Открыть/получить комнату (User → Doctor)
    [HttpPost("room")]
    [Authorize]
    public async Task<ActionResult<ChatRoomDto>> OpenRoom(CreateChatRoomDto dto)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
            return Unauthorized("Invalid user identity");

        var room = await _chat.OpenRoomAsync(userId, dto.DoctorId);
        return Ok(room);
    }

    // Список комнат у доктора
    [HttpGet("doctor/rooms")]
    [Authorize(Roles = Roles.Doctor)]
    public async Task<ActionResult<List<ChatRoomDto>>> GetDoctorRooms()
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var doctorId))
            return Unauthorized("Invalid user identity");

        var rooms = await _chat.ListDoctorRoomsAsync(doctorId);
        return Ok(rooms);
    }

    // История сообщений в комнате
    [HttpGet("room/{roomId}/history")]
    [Authorize]
    public async Task<ActionResult<List<ChatMessageDto>>> GetHistory(int roomId, [FromQuery] int take = 50)
    {
        var messages = await _chat.GetMessagesAsync(roomId, take);
        return Ok(messages);
    }
}