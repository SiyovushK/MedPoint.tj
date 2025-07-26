namespace Domain.DTOs.ChatDTOs;

public class CreateChatMessageDto
{
    public string RoomId { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}