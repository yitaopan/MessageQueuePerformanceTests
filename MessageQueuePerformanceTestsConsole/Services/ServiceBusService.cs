using Azure.Identity;
using Azure.Messaging.ServiceBus;
using MessageQueuePerformanceTestsConsole.Models;
using System.Diagnostics;

namespace MessageQueuePerformanceTestsConsole.Services
{
    public class ServiceBusService : IMessageQueueService
    {
        private const string SERVICE_BUS_NAME = "yitaotestsb";
        private const string SERVICE_BUS_NAMESAPCE = $"{SERVICE_BUS_NAME}.servicebus.windows.net";
        private const string SERVICE_BUS_QUEUE_NAME = "yitaotestqueuesb";
        private const string SERVICE_BUS_CONNECTION_STRING = "";

        private const int DEFAULT_DISCARD_MESSAGE_BATCH_SIZE = 100;

        public  async Task<TestResult> SendMessages(int messageCount = 1)
        {
            ServiceBusMessage message = new("Hello, World!");

            Stopwatch stopwatch = new();

            stopwatch.Start();
            //ServiceBusClient client = new(Environment.GetEnvironmentVariable("SERVICE_BUS_CONNECTION_STRING"));
            ServiceBusClient client = new(SERVICE_BUS_CONNECTION_STRING);
            ServiceBusSender sender = client.CreateSender(SERVICE_BUS_QUEUE_NAME);
            stopwatch.Stop();
            TimeSpan setupClientDuration = stopwatch.Elapsed;

            // Incase first message is slower
            stopwatch.Restart();
            await sender.SendMessageAsync(message);
            stopwatch.Stop();
            TimeSpan firstMessageDuration = stopwatch.Elapsed;

            stopwatch.Restart();
            //Parallel.ForEach(Enumerable.Range(0, messageCount), async i =>
            //{
            //    await sender.SendMessageAsync(message);
            //});
            List<Task> sendMessageTasks = [];
            for (int i = 0; i < messageCount; i++)
            {
                sendMessageTasks.Add(sender.SendMessageAsync(message));
            }
            await Task.WhenAll(sendMessageTasks);
            stopwatch.Stop();
            TimeSpan duration = stopwatch.Elapsed;

            return new()
            {
                TotalMessageCount = messageCount + 1,
                SetupClientDuration = setupClientDuration,
                FirstMessageDuration = firstMessageDuration,
                Duration = duration,
            };
        }

        public async Task<string> DiscardAllMessages()
        {
            //ServiceBusClient client = new(Environment.GetEnvironmentVariable("SERVICE_BUS_CONNECTION_STRING"));
            ServiceBusClient client = new(SERVICE_BUS_CONNECTION_STRING);
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
                    return $"{messageCount} messages discarded, duration: {duration}.";
                }
                foreach (var message in await receiver.ReceiveMessagesAsync(DEFAULT_DISCARD_MESSAGE_BATCH_SIZE))
                {
                    messageCount++;
                    await receiver.CompleteMessageAsync(message);
                }
            } while (true);
        }
    }
}
