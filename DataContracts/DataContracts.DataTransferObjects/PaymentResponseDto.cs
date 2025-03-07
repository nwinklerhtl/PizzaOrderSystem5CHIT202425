namespace DataContracts.DataTransferObjects;

public class PaymentResponseDto
{
    public Guid OrderId { get; set; }
    public DateTimeOffset PaidAt { get; set; }
}