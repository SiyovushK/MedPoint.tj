namespace Domain.Entities;

public class Chat
{
    public string ChatId { get; set; } = string.Empty;
    public int User1Id { get; set; }
    public int User2Id { get; set; }
    public string User1Name { get; set; } = string.Empty;
    public string User2Name { get; set; } = string.Empty;
    public DateTime LastMessageAt { get; set; }
    public string LastMessagePreview { get; set; } = string.Empty;
    public int UnreadCount { get; set; }
}