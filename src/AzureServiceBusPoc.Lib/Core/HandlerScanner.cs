using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace AzureServiceBusPoc.Lib.Core
{
    public interface IMessageHandler { }

    public interface IHandleMessages<in T> : IMessageHandler
    {
        Task Handle(T message, IMessageContext context, CancellationToken cancellationToken);
    }

    internal class HandlerScanner
    {
        public int HandlerCount { get; private set; }

        public void ScanAndRegisterHandlers(IServiceCollection services, params Assembly[] assembliesToScan)
        {
            var toScan = !assembliesToScan.Any() ? AppDomain.CurrentDomain.GetAssemblies() : assembliesToScan;
            var handlerTypes = toScan
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(IMessageHandler).IsAssignableFrom(t)
                            && !t.IsInterface
                            && !t.IsAbstract);

            foreach (var handlerType in handlerTypes)
            {
                GetTypesHandled(handlerType).ForEach(ht =>
                {
                    var i = typeof(IHandleMessages<>);
                    var serviceType = i.MakeGenericType(new[] { ht });

                    services.AddTransient(serviceType, p =>
                    {
                        var handler = ActivatorUtilities.CreateInstance(p, handlerType);
                        return handler;
                    });

                    HandlerCount++;
                });
            }
        }

        private static List<Type> GetTypesHandled(Type handler)
        {
            var messageTypes = handler
                .GetInterfaces()
                .Where(i => i.IsGenericType)
                .Select(i => i.GetGenericArguments().Single())
                .ToList();

            return messageTypes;
        }
    }
}
