using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Services.User.Domain;

[Table("order_requests")]
public class OrderRequest
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }
    
    [Column("customer_name")]
    [Required]
    public string CustomerName { get; set; }
    
    [Column("customer_address")]
    [Required]
    public string CustomerAddress { get; set; }

    public List<OrderRequestItem> Items { get; set; } = [];
}