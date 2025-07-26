namespace Domain.DTOs.ChatDTOs;

public class ChatRoomDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int DoctorId { get; set; }
    public DateTime CreatedAt { get; set; }
}