using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataContracts.Messages.Base;

namespace Infrastructure.Messaging.Outbox.Domain;

[Table("outbox")]
public class OutboxMessage
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("message")]
    public string Message { get; set; }
    
    [Column("message_type")]
    public string MessageType { get; set; }
    
    [Column("sent_at")]
    public DateTimeOffset? SentAt { get; set; }

    public static OutboxMessage FromMessage(AMessage msg) =>
        new()
        {
            MessageType = msg.MessageType(),
            Message = msg.SerializeToJson().ToString()
        };
}