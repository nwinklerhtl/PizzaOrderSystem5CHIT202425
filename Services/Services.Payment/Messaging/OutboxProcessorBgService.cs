using Infrastructure.Messaging.Interfaces;
using Infrastructure.Messaging.Outbox;
using Microsoft.EntityFrameworkCore;
using Services.Payment.Db;

namespace Services.Payment.Messaging;

public class OutboxProcessorBgService(
    IDbContextFactory<PaymentDbContext> dbContextFactory,
    IMessageSender messageSender) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await new OutboxProcessor()
            .Process(dbContextFactory.CreateDbContext, messageSender, stoppingToken);
    }
}