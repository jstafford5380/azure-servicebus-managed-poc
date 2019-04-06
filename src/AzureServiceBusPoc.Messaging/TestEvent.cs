using AzureServiceBusPoc.Lib;
using AzureServiceBusPoc.Lib.Shared;

namespace AzureServiceBusPoc.Messaging
{
    public class TestEvent : IEvent
    {
        public string Message { get; set; }
    }

    public class TestCommand : ICommand
    {
        public string Message { get; set; }
    }

    public class RewireMeCommand : ICommand
    {
        public string Message { get; set; }
    }
}