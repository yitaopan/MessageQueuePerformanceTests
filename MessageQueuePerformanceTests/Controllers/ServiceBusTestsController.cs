using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MessageQueuePerformanceTests.Controllers
{
    [ApiController]
    public class ServiceBusTestsController : ControllerBase
    {
        private const string SERVICE_BUS_NAME = "yitaotestsb";
        private const string SERVICE_BUS_NAMESAPCE = $"{SERVICE_BUS_NAME}.servicebus.windows.net";
        private const string SERVICE_BUS_QUEUE_NAME = "yitaotestqueuesb";

        private const int DEFAULT_DISCARD_MESSAGE_BATCH_SIZE = 10;

        [HttpPost("servicebus/send")]
        public async Task<IActionResult> SendServiceBusMessages([FromQuery] int messageCount = 1)
        {
            ServiceBusMessage message = new("Hello, World!");

            ServiceBusClientOptions clientOptions = new()
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets,
            };
            ServiceBusClient client = new(SERVICE_BUS_NAMESAPCE, new DefaultAzureCredential(), clientOptions);
            ServiceBusSender sender = client.CreateSender(SERVICE_BUS_QUEUE_NAME);

            // Incase first message is slower
            await sender.SendMessageAsync(message);

            List<Task> sendMessageTasks = [];
            for (int i = 0; i < messageCount; i++)
            {
                sendMessageTasks.Add(sender.SendMessageAsync(message));
            }

            Stopwatch stopwatch = new();
            stopwatch.Start();
            await Task.WhenAll(sendMessageTasks);
            stopwatch.Stop();
            TimeSpan duration = stopwatch.Elapsed;

            return Ok($"Duration for sending {messageCount} messages: {duration}");
        }

        [HttpPost("servicebus/discard/all")]
        public async Task<IActionResult> DiscardAllServiceBusMessages()
        {
            ServiceBusClientOptions clientOptions = new()
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets,
            };
            ServiceBusClient client = new(SERVICE_BUS_NAMESAPCE, new DefaultAzureCredential(), clientOptions);
            ServiceBusReceiver receiver = client.CreateReceiver(SERVICE_BUS_QUEUE_NAME);


            int messageCount = 0;
            Stopwatch stopwatch = new();
            stopwatch.Start();
            do
            {
                if (await receiver.PeekMessageAsync() == null)
                {
                    stopwatch.Stop();
                    TimeSpan duration = stopwatch.Elapsed;
                    return Ok($"{messageCount} messages discarded, duration: {duration}.");
                }
                foreach (var message in await receiver.ReceiveMessagesAsync(DEFAULT_DISCARD_MESSAGE_BATCH_SIZE))
                {
                    await receiver.CompleteMessageAsync(message);
                }
            } while (true);
        }
    }
}
