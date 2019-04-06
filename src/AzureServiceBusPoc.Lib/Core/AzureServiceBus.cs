using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureServiceBusPoc.Lib.Configuration;
using AzureServiceBusPoc.Lib.Shared;
using Microsoft.Azure.ServiceBus;
using XidNet;

namespace AzureServiceBusPoc.Lib.Core
{
    // TODO: create message abstraction so this isn't azure-specific?

    public sealed class AzureServiceBus : IServiceBus
    {
        private readonly ServiceBusConnection _connection;
        private readonly RetryExponential _retryPolicy;
        private readonly IServiceProvider _services;

        public ServiceBusConfiguration Configuration { get; }

        public AzureServiceBus(ServiceBusConnection connection, ServiceBusConfiguration config, IServiceProvider services)
        {
            _connection = connection;
            Configuration = config;
            _services = services;
            _retryPolicy = new RetryExponential(
                TimeSpan.FromSeconds(1),
                TimeSpan.FromMinutes(1),
                10);
        }

        public async Task SendAsync<T>(T message, string destination, SendOptions sendOptions = null)
            where T : ICommand
        {
            var messageBody = CreateNewMessage(message);
            await SendMessageAsync(messageBody, destination, ChannelType.Queue);
        }

        public async Task PublishAsync<T>(T message, string topic, SendOptions options = null)
            where T : IEvent
        {
            var messageBody = CreateNewMessage(message);
            await SendMessageAsync(messageBody, topic, ChannelType.Topic);
        }

        public Task SendLocalAsync<T>(T message) where T : ICommand
        {
            var messageBody = CreateNewMessage(message);
            return SendMessageAsync(messageBody, Configuration.EndpointName, ChannelType.Queue);
        }

        public Task SendMessageAsync(Message message, string destination, ChannelType type)
        {
            EnsureCanSend();
            message.UserProperties[HeaderTypes.MessageIntent] = new Dictionary<ChannelType, MessageIntent>
            {
                [ChannelType.Topic] = MessageIntent.Event,
                [ChannelType.Queue] = MessageIntent.Command
            }[type].ToString();

            var work = Task.CompletedTask;
            switch (type)
            {
                case ChannelType.Queue:
                    work = new QueueClient(_connection, destination, ReceiveMode.PeekLock, _retryPolicy).SendAsync(message);
                    break;
                case ChannelType.Topic:
                    work = new TopicClient(_connection, destination, _retryPolicy).SendAsync(message);
                    break;
            }

            return work;
        }

        private void EnsureCanSend()
        {
            if(!Configuration.CanSend)
                throw new InvalidOperationException("This client is not configured to send.");
        }

        private Message CreateNewMessage(object message, SendOptions options = null)
        {
            var jsonBytes = SerializationHelper.GetJsonBytes(message);
            var msg = new Message(jsonBytes) { ContentType = "application/json" };

            if (options != null)
            {
                foreach (var header in options.Headers)
                {
                    msg.UserProperties.Add(header.Key, header.Value);
                }
            }

            msg.ReplyTo = Configuration.EndpointName;
            msg.MessageId = Xid.NewXid().ToString();
            msg.CorrelationId = msg.MessageId;
            msg.UserProperties[HeaderTypes.ConversationId] = Xid.NewXid().ToString();
            msg.UserProperties[HeaderTypes.EnclosedType] = message.GetType().FullName;

            return msg;
        }

        public IServiceBusConsumer ConsumeTopic(string topicName, string subscription)
        {
            var consumer = new AzureServiceBusConsumer(_connection, _retryPolicy, _services);
            return consumer.ConsumeTopic(topicName, subscription);
        }

        public IServiceBusConsumer ConsumeQueue(string queueName)
        {
            var consumer = new AzureServiceBusConsumer(_connection, _retryPolicy, _services);
            return consumer.ConsumeQueue(queueName);
        }

        public void Dispose()
        {
            _connection.CloseAsync().Wait();
        }
    }
}
