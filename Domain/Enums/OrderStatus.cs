namespace Domain.Enums;

public enum OrderStatus
{
    Pending = 0,
    Active = 1,
    NotAccepted = 2,
    CancelledByUser = 3,
    CancelledByDoctor = 4,
    Finished = 5
}