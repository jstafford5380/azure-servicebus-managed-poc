using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AzureServiceBusPoc.Lib.Configuration;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace AzureServiceBusPoc.Lib.Core
{
    public class MessageDispatcher
    {
        private readonly IServiceBus _serviceBus;
        private readonly IServiceProvider _serviceProvider;

        public MessageDispatcher(IServiceProvider serviceProvider)
        {
            _serviceBus = serviceProvider.GetRequiredService<IServiceBus>();
            _serviceProvider = serviceProvider;
        }

        public async Task Dispatch(Message incomingMessage, CancellationToken cancellationToken)
        {
            var handlers = CreateHandlers(incomingMessage, out var messageBody);
            if (handlers == null || messageBody is JObject) return;

            var context = new MessageContext(_serviceBus, incomingMessage);
            
            var handleTasks = handlers.Select(async h =>
            {
                var method = h
                    .GetType()
                    .GetMethod("Handle", new[] { messageBody.GetType(), typeof(MessageContext), typeof(CancellationToken) });

                if (method == null) return;
                await (Task)method.Invoke(h, new[] { messageBody, context, cancellationToken });
            });

            await Task.WhenAll(handleTasks);
        }

        private List<object> CreateHandlers(Message message, out object messageBody)
        {
            IHandleTypeResolver resolver = null;
            if (_serviceBus.Configuration.HandleTypeResolverType != null)
                resolver = (IHandleTypeResolver) ActivatorUtilities.CreateInstance(
                    _serviceProvider,
                    _serviceBus.Configuration.HandleTypeResolverType);

            messageBody = DeserializeMessage(message, resolver);

            var handlerType = CreateHandlerType(messageBody.GetType());
            var handlers = _serviceProvider.GetServices(handlerType)?.ToList() ?? new List<object>();
            return handlers;
        }

        private static object DeserializeMessage(Message message, IHandleTypeResolver resolver)
        {
            // use two deserialization strategies. If resolver exists, attempt to use it
            // if no type is found, fallback to JsonConvert. If that can't find a valid
            // type, it will return JObject

            object messageBody = null;

            var enclosedType = message.UserProperties[HeaderTypes.EnclosedType] as string;
            if (resolver != null)
            {
                var alternativeType = resolver.UseTypeFor(enclosedType);
                if (alternativeType != null)
                    messageBody = SerializationHelper.GetObject(message.Body, alternativeType);
            }

            return messageBody ?? SerializationHelper.GetObject(message.Body);
        }

        private static Type CreateHandlerType(Type messageType)
        {
            var t = typeof(IHandleMessages<>);
            Type[] typeArgs = { messageType };
            var handlerType = t.MakeGenericType(typeArgs);
            return handlerType;
        }
    }
}

