using DataContracts.Messages.Base;

namespace DataContracts.Messages.ServiceMessages;

public class OrderReceived : AOrderIdMessage
{
    private const string DefaultSource = "https://user.pizza-order-system.com";

    public OrderReceived()
    {
        SourceUri = new Uri(DefaultSource);
    }

    public OrderReceived(AOrderIdMessage other) : base(other)
    {
        SourceUri = new Uri(DefaultSource);
    }

    public override string MessageType() => Constants.OrderReceivedV1;
    
    public decimal TotalValue { get; set; }
}