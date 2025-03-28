namespace DataContracts.DataTransferObjects;

public class OrderInfoDto
{
    public Guid OrderId { get; set; }
    public List<OrderInfoItemDto> OrderItems { get; set; } = [];
}

public class OrderInfoItemDto
{
    public string ArticleName { get; set; }
    public int Amount { get; set; }
}