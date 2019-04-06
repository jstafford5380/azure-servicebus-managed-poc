using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureServiceBusPoc.Lib.Shared;
using Microsoft.Azure.ServiceBus;
using XidNet;

namespace AzureServiceBusPoc.Lib.Core
{
    // TODO: create message abstraction so this isn't azure-specific

    public class MessageContext : IMessageContext
    {
        private readonly IServiceBus _serviceBus;
        private readonly Message _incomingMessage;

        public string MessageId => _incomingMessage.MessageId;

        public string CorrelationId => _incomingMessage.CorrelationId;

        public string ConversationId => _incomingMessage.UserProperties[HeaderTypes.ConversationId].ToString();

        public string Source => _incomingMessage.ReplyTo;

        public bool IsConversationStarter => MessageId.Equals(CorrelationId);


        public MessageContext(IServiceBus serviceBus, Message incomingMessage)
        {
            _serviceBus = serviceBus;
            _incomingMessage = incomingMessage;
        }

        public Task SendAsync<T>(T message, string destination, SendOptions options = null)
            where T : ICommand
        {
            var outgoingMessage = PrepareMessage(message, options);
            return _serviceBus.SendMessageAsync(outgoingMessage, destination, ChannelType.Queue);
        }

        public Task SendLocalAsync<T>(T message) where T : ICommand
        {
            return SendAsync(message, _serviceBus.Configuration.EndpointName);
        }

        public Task PublishAsync<T>(T message, string destination, SendOptions options = null)
            where T : IEvent
        {
            var outgoingMessage = PrepareMessage(message, options);
            return _serviceBus.SendMessageAsync(outgoingMessage, destination, ChannelType.Topic);
        }

        private Message PrepareMessage<T>(T message, SendOptions options)
        {
            var outgoingMessage = GetMessage(message);
            MergeHeaders(outgoingMessage, options?.Headers);
            return outgoingMessage;
        }

        private static void MergeHeaders(Message outgoingMessage, IDictionary<string, object> newHeaders)
        {
            if (newHeaders == null) return;

            foreach (var header in newHeaders)
                outgoingMessage.UserProperties[header.Key] = header.Value;
        }

        private Message GetMessage(object message)
        {
            var jsonBytes = SerializationHelper.GetJsonBytes(message);
            var newMessage = new Message(jsonBytes);

            // echo back user headers
            foreach (var header in _incomingMessage.UserProperties)
                newMessage.UserProperties.Add(header.Key, header.Value);

            newMessage.CorrelationId = _incomingMessage.MessageId;
            newMessage.MessageId = Xid.NewXid().ToString();
            newMessage.UserProperties[HeaderTypes.EnclosedType] = message.GetType().FullName;

            return newMessage;
        }
    }
}