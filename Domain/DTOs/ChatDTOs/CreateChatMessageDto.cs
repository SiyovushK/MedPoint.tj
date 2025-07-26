namespace Domain.DTOs.ChatDTOs;

public class CreateChatMessageDto
{
    public int RoomId { get; set; }
    public string Text { get; set; } = string.Empty;
}