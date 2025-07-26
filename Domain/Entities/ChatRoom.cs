namespace Domain.Entities;

public class ChatRoom
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int DoctorId { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<ChatMessage> Messages { get; set; }
}