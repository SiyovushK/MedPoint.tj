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
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ChatController(IChatService chatService, IHttpContextAccessor httpContextAccessor)
    {
        _chatService = chatService;
        _httpContextAccessor = httpContextAccessor;
    }

    private int GetUserIdFromToken()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim);
    }

    [HttpPost("Send-message")]
    public async Task<IActionResult> SendMessage([FromQuery] int? doctorId, [FromQuery] int? chatId, [FromBody] string text)
    {
        var senderId = GetUserIdFromToken();
        var result = await _chatService.SendMessageAsync(senderId, doctorId, chatId, text);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("Chat-messages/{chatId}")]
    public async Task<ActionResult<Response<List<MessageDto>>>> ChatMessages(int chatId)
    {
        var userId = GetUserIdFromToken();
        var result = await _chatService.GetChatMessagesAsync(chatId, userId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("User-chats")]
    public async Task<ActionResult<Response<List<ChatDto>>>> Chats()
    {
        var userId = GetUserIdFromToken();
        var result = await _chatService.GetUserChatsAsync(userId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("doctor-chats")]
    [Authorize(Roles = Roles.Doctor)]
    public async Task<IActionResult> GetDoctorChats()
    {
        var doctorId = GetUserIdFromToken();
        var result = await _chatService.GetDoctorChatsAsync(doctorId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("Chat-info/{chatId}")]
    public async Task<ActionResult<Response<ChatInfoDto>>> ChatInfo(int chatId)
    {
        var userId = GetUserIdFromToken();
        var result = await _chatService.GetChatInfoAsync(chatId, userId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("mark-read/{messageId}")]
    public async Task<ActionResult<Response<bool>>> MarkAsRead(int messageId)
    {
        var userId = GetUserIdFromToken();
        var result = await _chatService.MarkAsReadAsync(messageId, userId);
        return StatusCode(result.StatusCode, result);
    }
}