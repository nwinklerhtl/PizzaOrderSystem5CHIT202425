using CloudNative.CloudEvents;
using DataContracts.Messages;
using DataContracts.Messages.ServiceMessages;
using Infrastructure.Messaging.Interfaces;
using Infrastructure.Messaging.Outbox.Domain;
using Infrastructure.Messaging.RabbitMq;
using Microsoft.EntityFrameworkCore;
using Services.Payment.ApiRequestHandlers;
using Services.Payment.Db;
using Services.Payment.Domain;
using Services.Payment.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("PaymentDbConnection")
                       ?? throw new InvalidOperationException("Connection string not found");

builder.Services.AddDbContextFactory<PaymentDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddSingleton<IMessageSender>(_ =>
    RabbitMqMessagingFactory.CreateSenderAsync(Constants.ExchangeName).GetAwaiter().GetResult());

builder.Services.AddHostedService<OutboxProcessorBgService>();

var app = builder.Build();

var scope = app.Services.CreateScope();
await using var dbContext = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();
var connected = false;
while (!connected)
{
    if (!dbContext.Database.CanConnect())
    {
        await Task.Delay(1_000);
        continue;
    }
    
    dbContext.Database.Migrate();
    connected = true;
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapPost("/payment", PaymentRequesthandler.HandlePayment)
    .WithName("Payment")
    .WithOpenApi();

await RabbitMqMessagingFactory.CreateReceiverAsync<OrderReceived>(
    Constants.ExchangeName,
    async (cloudEvent, orderReceived) =>
    {
        dbContext.Payments.Add(new Payment()
        {
            OrderId = orderReceived.OrderId,
            PaymentAmount = orderReceived.TotalValue
        });

        dbContext.Outbox.Add(OutboxMessage.FromMessage(
            new Notification()
            {
                OrderId = orderReceived.OrderId,
                Title = "Payment pending",
                Message = $"We are waiting for your payment of {orderReceived.TotalValue:C}"
            }));

        await dbContext.SaveChangesAsync();
    });


app.Run();