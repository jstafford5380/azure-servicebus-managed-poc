using System.Collections.Generic;

namespace AzureServiceBusPoc.Lib.Configuration
{
    public class ConsumerContainer
    {
        private readonly List<IServiceBusConsumer> _consumers = new List<IServiceBusConsumer>();

        public void AddConsumer(IServiceBusConsumer consumer)
        {
            _consumers.Add(consumer);
        }

    }
}