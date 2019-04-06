using System;
using System.Collections.Generic;
using AzureServiceBusPoc.Lib.Configuration;
using AzureServiceBusPoc.Lib.Core;
using AzureServiceBusPoc.Messaging;

namespace AzureServiceBusPoc.App1
{
    public class TestResolver : IHandleTypeResolver
    {
        private readonly Dictionary<string, Type> _resolver = new Dictionary<string, Type>();

        public TestResolver()
        {
            var fullname = typeof(RewireMeCommand).FullName;
            _resolver.Add(fullname, typeof(RewiredCommand));
        }

        public Type UseTypeFor(string enclosedMessageType)
        {
            return _resolver.ContainsKey(enclosedMessageType) 
                ? _resolver[enclosedMessageType] 
                : null;
        }
    }
}