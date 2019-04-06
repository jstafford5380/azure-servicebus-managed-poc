using System;
using System.Reflection;
using AzureServiceBusPoc.Lib.Core;
using Microsoft.Extensions.Configuration;

namespace AzureServiceBusPoc.Lib.Configuration
{
    public class ServiceBusConfigurationBuilder
    {
        private string _connectionString;
        private string _endpointName;
        private int _maxConcurrency = Environment.ProcessorCount;
        private bool _receiveOnly;
        private bool _sendOnly;
        private Type _typeResolver;

        public Assembly[] IncludeInScan { get; private set; }

        public Action<ISubscriptionBuilder> StartSubscriptions { get; private set; }
        
        public ServiceBusConfigurationBuilder WithConnectionString(string connectionString)
        {
            _connectionString = connectionString;
            return this;
        }

        public ServiceBusConfigurationBuilder WithEndpointName(string endpointName)
        {
            _endpointName = endpointName;
            return this;
        }

        public ServiceBusConfigurationBuilder UseAppSettings(IConfiguration appSettings)
        {
            _connectionString = appSettings["AzureServiceBus:ConnectionString"];
            _endpointName = appSettings["AzureServiceBus:EndpointName"];

            return this;
        }

        public ServiceBusConfigurationBuilder ScanAssemblies(params Assembly[] assembliesToScan)
        {
            IncludeInScan = assembliesToScan;
            return this;
        }

        public ServiceBusConfigurationBuilder WithSubscriptions(Action<ISubscriptionBuilder> builder)
        {
            StartSubscriptions = builder;
            return this;
        }

        public ServiceBusConfigurationBuilder LimitConcurrencyTo(int maxConcurrency)
        {
            _maxConcurrency = maxConcurrency;
            return this;
        }

        public ServiceBusConfigurationBuilder UseTypeResolver<T>()
            where T : IHandleTypeResolver
        {
            _typeResolver = typeof(T);
            return this;
        }

        public void SendOnly()
        {
            _receiveOnly = false;
            _sendOnly = true;
        }

        public void ReceiveOnly()
        {
            _sendOnly = false;
            _receiveOnly = true;
        }

        public ServiceBusConfiguration Build()
        {
            if (string.IsNullOrEmpty(_connectionString))
                throw new ArgumentException("ConnectionString is required");

            if(IsReceiver && string.IsNullOrEmpty(_endpointName))
                throw new ArgumentException("If this bus can receive messages, then EndpointName is required");

            return new ServiceBusConfiguration
            {
                HandleTypeResolverType = _typeResolver,
                CanSend = IsSender,
                CanReceive = IsReceiver,
                ConnectionString = _connectionString,
                EndpointName = _endpointName,
                MaxConcurrency = _maxConcurrency
            };
        }

        private bool IsReceiver => _receiveOnly || !_receiveOnly && !_sendOnly;

        private bool IsSender => _sendOnly || !_sendOnly && !_receiveOnly;
    }
}