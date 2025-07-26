namespace Domain.Entities;

public class ChatMessage
{
    public int Id { get; set; }
    public string RoomId { get; set; } = string.Empty;           // Идентификатор комнаты (например, doctor‑123 или support)
    public string SenderId { get; set; } = string.Empty;         // Id пользователя, отправившего сообщение
    public string SenderName { get; set; } = string.Empty;       // Для отображения имени
    public string Text { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
}