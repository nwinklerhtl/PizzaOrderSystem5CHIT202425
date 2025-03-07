using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Services.User.Domain;

[Table("order_request_items")]
public class OrderRequestItem
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Column("article_name")]
    [Required]
    public string ArticleName { get; set; }
    
    [Column("article_price")]
    [Required]
    public decimal ArticlePrice { get; set; }
    
    [Column("amount")]
    [Required]
    public int Amount { get; set; }
}