using System.Security.Claims;
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
    public async Task<ActionResult<Response<int>>> SendMessage([FromQuery] int doctorId, [FromBody] string text)
    {
        var userId = GetUserIdFromToken();
        var result = await _chatService.SendMessageAsync(userId, doctorId, text);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("Chat-messages/{chatId}")]
    public async Task<ActionResult<Response<List<MessageDto>>>> ChatMessages(int chatId)
    {
        var userId = GetUserIdFromToken();
        var result = await _chatService.GetChatMessagesAsync(chatId, userId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("Chats-list")]
    public async Task<ActionResult<Response<List<ChatDto>>>> Chats()
    {
        var userId = GetUserIdFromToken();
        var result = await _chatService.GetUserChatsAsync(userId);
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