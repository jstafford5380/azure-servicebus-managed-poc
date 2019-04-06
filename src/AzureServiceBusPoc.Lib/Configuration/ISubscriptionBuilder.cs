namespace AzureServiceBusPoc.Lib.Configuration
{
    public interface ISubscriptionBuilder
    {
        void ConsumeTopic(string topicName, string subscriptionName);

        void ConsumeQueue(string queueName);
    }
}