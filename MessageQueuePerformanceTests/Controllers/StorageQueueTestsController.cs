using Azure.Identity;
using Azure.Storage.Queues;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MessageQueuePerformanceTests.Controllers
{
    [ApiController]
    public class StorageQueueTestsController : Controller
    {
        private const string STORAGE_ACCOUNT_NAME = "yitaoteststandardsa";
        private const string STORAGE_QUEUE_NAME = "yitaotestqueuesa";
        private const string STORAGE_QUEUE_URI = $"https://{STORAGE_ACCOUNT_NAME}.queue.core.windows.net/{STORAGE_QUEUE_NAME}";

        private const int DEFAULT_DISCARD_MESSAGE_BATCH_SIZE = 10;

        [HttpPost("storagequeue/send")]
        public async Task<IActionResult> SendStorageQueue([FromQuery] int messageCount = 1)
        {
            string message = "Hello, World!";

            QueueClient queueClient = new(new Uri(STORAGE_QUEUE_URI), new DefaultAzureCredential());
            await queueClient.CreateIfNotExistsAsync();

            await queueClient.SendMessageAsync(message);

            List<Task> sendMessageTasks = [];
            for (int i = 0; i < messageCount; i++)
            {
                sendMessageTasks.Add(queueClient.SendMessageAsync(message));
            }

            Stopwatch stopwatch = new();
            stopwatch.Start();
            await Task.WhenAll(sendMessageTasks);
            stopwatch.Stop();
            TimeSpan duration = stopwatch.Elapsed;

            return Ok($"Duration for sending {messageCount} messages: {duration}");
        }

        [HttpPost("storagequeue/discard/all")]
        public async Task<IActionResult> DiscardAllStorageQueueMessages()
        {
            QueueClient queueClient = new(new Uri(STORAGE_QUEUE_URI), new DefaultAzureCredential());
            await queueClient.CreateIfNotExistsAsync();

            do
            {
                if ((await queueClient.PeekMessageAsync()).Value == null)
                {
                    return Ok("All messages discarded.");
                }
                foreach (var message in (await queueClient.ReceiveMessagesAsync(maxMessages: DEFAULT_DISCARD_MESSAGE_BATCH_SIZE)).Value)
                {
                    await queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);
                }
            } while (true);
        }
    }
}
