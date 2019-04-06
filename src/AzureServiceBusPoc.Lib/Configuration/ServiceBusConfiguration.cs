using System;

namespace AzureServiceBusPoc.Lib.Configuration
{
    public class ServiceBusConfiguration
    {
        public string ConnectionString { get; set; }

        public string EndpointName { get; set; }

        public int MaxConcurrency { get; set; }

        public bool CanReceive { get; set; }

        public bool CanSend { get; set; }

        public Type HandleTypeResolverType { get; set; }
    }
}