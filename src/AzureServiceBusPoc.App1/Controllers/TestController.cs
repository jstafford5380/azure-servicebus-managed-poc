using System.Threading.Tasks;
using AzureServiceBusPoc.Lib.Core;
using AzureServiceBusPoc.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace AzureServiceBusPoc.App1.Controllers
{
    [Route("")]
    public class TestController : Controller
    {
        private readonly IServiceBus _bus;

        public TestController(IServiceBus bus)
        {
            _bus = bus;
        }

        [HttpGet, Route("test")]
        public async Task<IActionResult> TestBus()
        {
            var testCommand = new TestCommand {Message = "Hello queue!"};
            var testEvent = new TestEvent {Message = "Hello topic!"};
            var testRewire = new RewireMeCommand {Message = "I should be rewired!"};

            await _bus.SendAsync(testCommand, "app1");
            await _bus.SendLocalAsync(testRewire);
            await _bus.SendLocalAsync(testCommand);
            await _bus.PublishAsync(testEvent, "test-foo");

            return Ok();
        }
    }
}
