namespace DataContracts.DataTransferObjects;

public class OrderItemDto
{
    public string ArticleName { get; set; }
    public decimal ArticlePrice { get; set; }
    public int Amount { get; set; }
}