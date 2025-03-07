namespace DataContracts.DataTransferObjects;

public class PlaceOrderRequestDto
{
    public string CustomerName { get; set; }
    public string CustomerAddress { get; set; }
    public List<OrderItemDto> Items { get; set; }
}