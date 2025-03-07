using DataContracts.Messages.Base;

namespace Infrastructure.Messaging.Interfaces;

public interface IMessageSender : IDisposable, IAsyncDisposable
{
    Task SendMessageAsync(AMessage message);
}