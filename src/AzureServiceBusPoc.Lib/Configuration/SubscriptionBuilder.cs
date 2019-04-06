using AzureServiceBusPoc.Lib.Core;

namespace AzureServiceBusPoc.Lib.Configuration
{
    public class SubscriptionBuilder : ISubscriptionBuilder
    {
        private readonly IServiceBus _serviceBus;
        private readonly ConsumerContainer _container;

        public SubscriptionBuilder(IServiceBus serviceBus)
        {
            _serviceBus = serviceBus;
            _container = new ConsumerContainer();
        }

        public void ConsumeTopic(string topicName, string subscriptionName)
        {
            _container.AddConsumer(_serviceBus.ConsumeTopic(topicName, subscriptionName));
        }

        public void ConsumeQueue(string queueName)
        {
            _container.AddConsumer(_serviceBus.ConsumeQueue(queueName));
        }

        public ConsumerContainer Build() => _container;
    }
}