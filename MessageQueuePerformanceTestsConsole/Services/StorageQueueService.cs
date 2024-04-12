using Azure.Identity;
using Azure.Storage.Queues;
using MessageQueuePerformanceTestsConsole.Models;
using System.Diagnostics;

namespace MessageQueuePerformanceTestsConsole.Services
{
    public class StorageQueueService : IMessageQueueService
    {
        private const string STORAGE_ACCOUNT_NAME = "yitaoteststandardsa";
        private const string STORAGE_QUEUE_NAME = "yitaotestqueuesa";
        private const string STORAGE_QUEUE_URI = $"https://{STORAGE_ACCOUNT_NAME}.queue.core.windows.net/{STORAGE_QUEUE_NAME}";

        private const int DEFAULT_DISCARD_MESSAGE_BATCH_SIZE = 10;

        public async Task<TestResult> SendMessages(int messageCount = 1)
        {
            string message = "Hello, World!";

            Stopwatch stopwatch = new();

            stopwatch.Start();
            QueueClient queueClient = new(Environment.GetEnvironmentVariable("STORAGE_ACCOUNT_CONNECTION_STRING"), STORAGE_QUEUE_NAME);
            await queueClient.CreateIfNotExistsAsync();
            stopwatch.Stop();
            TimeSpan setupClientDuration = stopwatch.Elapsed;

            // Incase first message is slower
            stopwatch.Restart();
            await queueClient.SendMessageAsync(message);
            stopwatch.Stop();
            TimeSpan firstMessageDuration = stopwatch.Elapsed;

            stopwatch.Restart();
            //Parallel.ForEach(Enumerable.Range(0, messageCount), async i =>
            //{
            //    await queueClient.SendMessageAsync(message);
            //});
            List<Task> sendMessageTasks = [];
            for (int i = 0; i < messageCount; i++)
            {
                sendMessageTasks.Add(queueClient.SendMessageAsync(message));
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
            QueueClient queueClient = new(new Uri(STORAGE_QUEUE_URI), new DefaultAzureCredential(
                new DefaultAzureCredentialOptions()
                {
                    ManagedIdentityClientId = "965124b5-ff7a-4929-8301-4d16759229b4",
                }));
            await queueClient.CreateIfNotExistsAsync();

            int messageCount = 0;
            Stopwatch stopwatch = new();
            stopwatch.Start();
            do
            {
                if ((await queueClient.PeekMessageAsync()).Value == null)
                {
                    stopwatch.Stop();
                    TimeSpan duration = stopwatch.Elapsed;
                    return $"{messageCount} messages discarded, duration: {duration}.";
                }
                foreach (var message in (await queueClient.ReceiveMessagesAsync(maxMessages: DEFAULT_DISCARD_MESSAGE_BATCH_SIZE)).Value)
                {
                    messageCount++;
                    await queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);
                }
            } while (true);
        }
    }
}
