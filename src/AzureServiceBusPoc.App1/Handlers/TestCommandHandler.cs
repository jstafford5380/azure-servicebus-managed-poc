using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using AzureServiceBusPoc.Lib.Core;
using AzureServiceBusPoc.Lib.Shared;
using AzureServiceBusPoc.Messaging;

namespace AzureServiceBusPoc.App1.Handlers
{
    public class TestCommandHandler : IHandleMessages<TestCommand>, IHandleMessages<TestResponse>, IHandleMessages<RewiredCommand>
    {
        public async Task Handle(TestCommand message, IMessageContext context, CancellationToken cancellationToken)
        {
            OutputTrace(message.Message, context);
            var reply = new TestResponse {Message = "I'm a reply!"};
            await context.SendLocalAsync(reply);
        }

        public Task Handle(TestResponse message, IMessageContext context, CancellationToken cancellationToken)
        {
            OutputTrace(message.Message, context);
            return Task.CompletedTask;
        }
        
        public Task Handle(RewiredCommand message, IMessageContext context, CancellationToken cancellationToken)
        {
            var msg = $"REWIRED: {message.Message}";
            OutputTrace(msg, context);
            return Task.CompletedTask;
        }

        private static void OutputTrace(string message, IMessageContext context)
        {
            Trace.WriteLine($"Received: {context.MessageId}\n\tIn Response To: {context.CorrelationId}\n\tStartedConversation: {context.IsConversationStarter}\n\tConversation: {context.ConversationId}\n\t{message}");
        }
    }

    public class TestResponse : ICommand
    {
        public string Message { get; set; }
    }

    public class TestEventHandler : IHandleMessages<TestEvent>
    {
        public Task Handle(TestEvent message, IMessageContext context, CancellationToken cancellationToken)
        {
            Trace.WriteLine(message.Message);
            return Task.CompletedTask;
        }
    }
}
