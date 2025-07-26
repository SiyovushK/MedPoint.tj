namespace Domain.DTOs.ChatDTOs;

public class ChatMessageDto
{
    public int RoomId { get; set; }
    public int SenderId { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
}