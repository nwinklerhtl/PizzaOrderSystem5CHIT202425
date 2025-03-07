using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Services.Payment.Domain;

[Table("payments")]
public class Payment
{
    [Key]
    [Column("order_id")]
    public Guid OrderId { get; set; }
    
    [Column("payment_amount")]
    [Required]
    public decimal PaymentAmount { get; set; }
    
    [Column("paid_at")]
    public DateTimeOffset? PaidAt { get; set; }
}