using System;
using System.Threading.Tasks;
using AzureServiceBusPoc.Lib.Configuration;
using Microsoft.Azure.ServiceBus;

namespace AzureServiceBusPoc.Lib.Core
{
    public interface IServiceBus : IMessageSender, IDisposable
    {
        ServiceBusConfiguration Configuration { get; }

        Task SendMessageAsync(Message message, string destination, ChannelType type);

        IServiceBusConsumer ConsumeTopic(string topicName, string subscription);

        IServiceBusConsumer ConsumeQueue(string queueName);
    }
}