using DataContracts.Messages.Base;

namespace DataContracts.Messages.ServiceMessages;

public class Notification : AOrderIdMessage
{
    private const string DefaultSource = "https://notification.pizza-order-system.com";

    public Notification()
    {
        SourceUri = new Uri(DefaultSource);
    }

    public Notification(AOrderIdMessage other) : base(other)
    {
        SourceUri = new Uri(DefaultSource);
    }

    public override string MessageType() => Constants.NotificationV1;
    
    public string Title { get; set; }
    public string Message { get; set; }
}