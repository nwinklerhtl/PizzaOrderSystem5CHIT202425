namespace DataContracts.DataTransferObjects;

public class NotificationDto
{
    public string Title { get; set; }
    public string Message { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}