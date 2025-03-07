using Infrastructure.Messaging.Interfaces;
using Infrastructure.Messaging.Outbox;
using Microsoft.EntityFrameworkCore;
using Services.User.Db;

namespace Services.User.Messaging;

public class OutboxProcessorBgService(
    IDbContextFactory<UserDbContext> dbContextFactory,
    IMessageSender messageSender) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await new OutboxProcessor()
            .Process(dbContextFactory.CreateDbContext, messageSender, stoppingToken);
    }
}