using System;

namespace AzureServiceBusPoc.Lib.Core
{
    public interface IHandleTypeResolver
    {
        Type UseTypeFor(string enclosedMessageType);
    }
}