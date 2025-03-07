using Amqp;
using CloudNative.CloudEvents;
using DataContracts.DataTransferObjects;
using DataContracts.Messages;
using DataContracts.Messages.ServiceMessages;
using Infrastructure.Messaging.RabbitMq;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR;
using Services.Notification.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        ["application/octet-stream"]);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("allowWebUi", builder =>
    {
        builder
            .WithOrigins("http://localhost:5089")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseResponseCompression();
app.MapHub<NotificationHub>("/notification-hub");

app.UseCors("allowWebUi");

var scope = app.Services.CreateScope();
await RabbitMqMessagingFactory.CreateReceiverAsync<Notification>(
    Constants.ExchangeName,
    async (cloudEvent, notification) =>
    {
        var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();
        await hubContext.Clients.Group(notification.OrderId.ToString()).SendAsync("OrderUpdate", new NotificationDto()
        {
            Title = notification.Title,
            Message = notification.Message,
            CreatedAt = cloudEvent.Time ?? DateTimeOffset.UtcNow
        });
    });

app.Run();