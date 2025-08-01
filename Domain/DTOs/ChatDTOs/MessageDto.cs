namespace Domain.DTOs.ChatDTOs;

public class MessageDto
{
    public int Id { get; set; }
    public int SenderId { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public bool IsRead { get; set; }
}