using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AzureServiceBusPoc.Lib.Configuration;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace AzureServiceBusPoc.Lib.Core
{
    public class AzureServiceBusConsumer : IServiceBusConsumer
    {
        private readonly ServiceBusConnection _connection;
        private readonly RetryPolicy _retryPolicy;
        private readonly IServiceProvider _services;
        private ClientEntity _client;
        private readonly MessageHandlerOptions _handlerOptions;

        public AzureServiceBusConsumer(
            ServiceBusConnection connection, 
            RetryPolicy retryPolicy,
            IServiceProvider services)
        {
            _connection = connection;
            _retryPolicy = retryPolicy;
            _services = services;
            var busConfiguration = _services.GetRequiredService<IServiceBus>().Configuration;
            _handlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = busConfiguration.MaxConcurrency,
                AutoComplete = true
            };
        }

        public AzureServiceBusConsumer ConsumeQueue(string queueName)
        {
            var client = new QueueClient(_connection, queueName, ReceiveMode.PeekLock, _retryPolicy);
            
            client.RegisterMessageHandler(Handler, _handlerOptions);
            return this;
        }

        public AzureServiceBusConsumer ConsumeTopic(
            string topicName, 
            string subscriptionName)
        {
            var client = new SubscriptionClient(
                _connection, 
                topicName, 
                subscriptionName, 
                ReceiveMode.PeekLock, 
                _retryPolicy);

            _client = client;
            
            client.RegisterMessageHandler(Handler, _handlerOptions);

            return this;
        }

        private async Task Handler(Message msg, CancellationToken cancellationToken)
        {
            var dispatcher = new MessageDispatcher(_services);
            await dispatcher.Dispatch(msg, cancellationToken);

            //// queue messages need to be ack'd
            //if (_client is QueueClient qc && !cancellationToken.IsCancellationRequested)
            //    await qc.CompleteAsync(msg.SystemProperties.LockToken);
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs arg)
        {
            // TODO: figure out if I need to error handle here or if retry policy will do it
            // TODO: is there somewhere I can catch a "giveup" so I can move it to error queue?
            
            Trace.WriteLine(arg.Exception.Message);
            return Task.CompletedTask;
        }
    }
}