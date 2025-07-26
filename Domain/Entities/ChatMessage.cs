namespace Domain.Entities;

public class ChatMessage
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public ChatRoom Room { get; set; }
    public int SenderId { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
}