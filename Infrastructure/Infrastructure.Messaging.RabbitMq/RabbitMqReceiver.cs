using DataContracts.Messages.Base;
using Infrastructure.Messaging.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Infrastructure.Messaging.RabbitMq;

public class RabbitMqReceiver<T>(string exchange, string hostname = "localhost")
: IMessageReceiver<T>
where T : AMessage, new()
{
    private IConnection? _connection;
    private IChannel? _channel;
    private AsyncEventingBasicConsumer? _consumer;

    public event CloudEventMessageReceived<T>? OnMessageReceived;

    public async Task ConnectAsync()
    {
        var factory = new ConnectionFactory() { HostName = hostname };
        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        var startupAssembly = 
            System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name ?? "Receiver";
        var messageType = new T().MessageType();
        var queueName = $"{startupAssembly}_{messageType}";

        await _channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        await _channel.QueueBindAsync(
            queue: queueName,
            exchange: exchange,
            routingKey: messageType,
            arguments: null);

        _consumer = new AsyncEventingBasicConsumer(_channel);
        _consumer.ReceivedAsync += ConsumerOnReceivedAsync;

        await _channel.BasicConsumeAsync(queueName, autoAck: false, consumer: _consumer);
    }

    private async Task ConsumerOnReceivedAsync(object sender, BasicDeliverEventArgs eventArgs)
    {
        var (cloudEvent, message) = AMessage.Deserialize<T>(eventArgs.Body.ToArray());

        try
        {
            if (OnMessageReceived is not null && message is not null)
            {
                await OnMessageReceived(cloudEvent, message);
            }

            if (_channel != null)
            {
                await _channel.BasicAckAsync(eventArgs.DeliveryTag, false);
            }
        }
        catch
        {
            // TODO: add DLQ
            await _channel!.BasicNackAsync(eventArgs.DeliveryTag, false, true);
        }
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