using DataContracts.Messages;
using Infrastructure.Messaging.Interfaces;
using Infrastructure.Messaging.RabbitMq;
using Microsoft.EntityFrameworkCore;
using Services.User.Db;
using Services.User.Messaging;
using Services.User.RequestHandlers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("UserDbConnection")
                       ?? throw new InvalidOperationException("Connection string not found");

builder.Services.AddDbContextFactory<UserDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddSingleton<IMessageSender>(_ =>
    RabbitMqMessagingFactory.CreateSenderAsync(Constants.ExchangeName).GetAwaiter().GetResult());

builder.Services.AddHostedService<OutboxProcessorBgService>();

var app = builder.Build();

var scope = app.Services.CreateScope();
await using var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
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

app.MapPost("/order", OrderRequestHandlers.HandlePlaceOrderRequest)
    .WithName("Place Order")
    .WithOpenApi();

app.Run();
