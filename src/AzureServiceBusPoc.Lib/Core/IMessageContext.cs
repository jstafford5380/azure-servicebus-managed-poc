using System.Threading.Tasks;
using AzureServiceBusPoc.Lib.Shared;

namespace AzureServiceBusPoc.Lib.Core
{
    public interface IMessageContext : IMessageSender
    {
        string MessageId { get; }

        string CorrelationId { get; }

        string ConversationId { get; }

        string Source { get; }

        bool IsConversationStarter { get; }
    }

    public interface IMessageSender
    {
        Task SendAsync<T>(T message, string destination, SendOptions options = null) where T : ICommand;

        Task PublishAsync<T>(T message, string destination, SendOptions options = null) where T : IEvent;

        Task SendLocalAsync<T>(T message) where T : ICommand;
    }
}