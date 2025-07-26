using Domain.DTOs.ChatDTOs;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpGet("history/{roomId}")]
    public async Task<ActionResult<List<ChatMessageDto>>> GetHistory(string roomId, [FromQuery] int take = 50)
    {
        var msgs = await _chatService.GetHistoryAsync(roomId, take);
        return Ok(msgs);
    }
}