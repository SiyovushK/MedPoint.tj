namespace Domain.DTOs.ChatDTOs;

public class ChatDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int DoctorId { get; set; }
    public DateTime LastMessageSentAt { get; set; }
    public string LastMessage { get; set; } = string.Empty;
    public int UnreadMessages { get; set; }
}