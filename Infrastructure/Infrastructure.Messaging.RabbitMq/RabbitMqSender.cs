using DataContracts.Messages.Base;
using Infrastructure.Messaging.Interfaces;
using Infrastructure.Messaging.RabbitMq.Exceptions;
using RabbitMQ.Client;

namespace Infrastructure.Messaging.RabbitMq;

public class RabbitMqSender(string exchange, string hostname = "localhost") : IMessageSender
{
    private IConnection? _connection;
    private IChannel? _channel;

    public async Task ConnectAsync()
    {
        var factory = new ConnectionFactory() { HostName = hostname };
        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        await _channel.ExchangeDeclareAsync(exchange, ExchangeType.Topic);
    }

    public async Task SendMessageAsync(AMessage message)
    {
        if (_channel is null)
        {
            throw new NotConnectedException();
        }

        await _channel.BasicPublishAsync(
            exchange: exchange,
            routingKey: message.MessageType(),
            mandatory: false,
            basicProperties: new BasicProperties(),
            body: message.Serialize());
    }
    
    public void Dispose()
    {
        _connection?.Dispose();
        _channel?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection != null) await _connection.DisposeAsync();
        if (_channel != null) await _channel.DisposeAsync();
    }
}