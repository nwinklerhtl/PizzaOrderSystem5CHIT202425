using DataContracts.DataTransferObjects;
using DataContracts.Messages;
using DataContracts.Messages.ServiceMessages;
using Infrastructure.Messaging.Interfaces;
using Infrastructure.Messaging.RabbitMq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("userClient", c =>
{
    c.BaseAddress = new Uri("http://localhost:5099");
});

var app = builder.Build();

var scope = app.Services.CreateScope();
var clientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
IMessageSender sender = await RabbitMqMessagingFactory.CreateSenderAsync(Constants.ExchangeName);

await RabbitMqMessagingFactory.CreateReceiverAsync<PaymentReceived>(
    Constants.ExchangeName,
    async (cloudEvent, paymentReceived) =>
    {
        using var httpClient = clientFactory.CreateClient("userClient");
        var orderInfo = await httpClient.GetFromJsonAsync<OrderInfoDto>($"/users-service/order/{paymentReceived.OrderId}");

        // TODO: error handling if no order information can be retrieved
        
        await sender.SendMessageAsync(new Notification()
        {
            OrderId = paymentReceived.OrderId,
            Title = "Preparing order",
            Message = "Our finest cooks will prepare your order now ...\n"
                + string.Join("\n", orderInfo.OrderItems.Select(i => $"{i.Amount} x {i.ArticleName}"))
        });

        await Task.Delay(5_000);
        
        // TODO: Send OrderPrepared Event instead
        await sender.SendMessageAsync(new Notification()
        {
            OrderId = paymentReceived.OrderId,
            Title = "Order prepared",
            Message = "Your order is ready for delivery."
        });
    });

app.Run();