using AzureServiceBusPoc.Lib.Core;

namespace AzureServiceBusPoc.Lib.Configuration
{
    public interface IServiceBusConsumer
    {
        AzureServiceBusConsumer ConsumeTopic(
            string topicName, 
            string subscriptionName);

        AzureServiceBusConsumer ConsumeQueue(string queueName);
    }
}