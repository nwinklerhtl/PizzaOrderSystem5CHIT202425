using DataContracts.Messages.Base;

namespace Infrastructure.Messaging.Interfaces;

public interface IMessageReceiver<out T> : IDisposable, IAsyncDisposable where T : AMessage, new()
{
    event CloudEventMessageReceived<T>? OnMessageReceived;
}