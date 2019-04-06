using System;
using AzureServiceBusPoc.Lib.Core;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.DependencyInjection;

namespace AzureServiceBusPoc.Lib.Configuration
{
    public static class ServiceBusInstaller
    {
        public static void AddAzureServiceBus(this IServiceCollection services, 
            Action<ServiceBusConfigurationBuilder> builder)
        {
            var configBuilder = new ServiceBusConfigurationBuilder();
            builder(configBuilder);
            var config = configBuilder.Build();

            var scanner = new HandlerScanner();
            scanner.ScanAndRegisterHandlers(services, configBuilder.IncludeInScan);
            
            var connectionBuilder = new ServiceBusConnectionStringBuilder(config.ConnectionString);
            var connection = new ServiceBusConnection(connectionBuilder);

            services.AddSingleton(connection);
            services.AddSingleton<IServiceBus>(p => new AzureServiceBus(connection, config, p));

            if (configBuilder.StartSubscriptions == null) return;

            var bus = services.BuildServiceProvider().GetRequiredService<IServiceBus>();
            var subscriptionBuilder = new SubscriptionBuilder(bus);

            if(config.CanReceive)
                subscriptionBuilder.ConsumeQueue(config.EndpointName);

            configBuilder.StartSubscriptions(subscriptionBuilder);

            var container = subscriptionBuilder.Build();
            services.AddSingleton(container);
        }
    }
}
