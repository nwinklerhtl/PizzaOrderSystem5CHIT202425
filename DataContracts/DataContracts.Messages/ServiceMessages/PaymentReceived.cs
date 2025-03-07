using DataContracts.Messages.Base;

namespace DataContracts.Messages.ServiceMessages;

public class PaymentReceived: AOrderIdMessage
{
    private const string DefaultSource = "https://payment.pizza-order-system.com";

    public PaymentReceived()
    {
        SourceUri = new Uri(DefaultSource);
    }

    public PaymentReceived(AOrderIdMessage other) : base(other)
    {
        SourceUri = new Uri(DefaultSource);
    }

    public override string MessageType() => Constants.PaymentReceivedV1;
}