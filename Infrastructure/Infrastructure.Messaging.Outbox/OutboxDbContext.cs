using Infrastructure.Messaging.Outbox.Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Messaging.Outbox;

public class OutboxDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<OutboxMessage> Outbox { get; set; }
}