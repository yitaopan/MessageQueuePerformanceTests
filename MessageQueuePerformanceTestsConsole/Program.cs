using MessageQueuePerformanceTestsConsole.Models;
using MessageQueuePerformanceTestsConsole.Services;

namespace MessageQueuePerformanceTestsConsole
{
    internal class Program
    {
        private const int DEFAULT_TEST_TIMES = 20;

        static async Task<int> Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine($"Please provide commands.");
                return 1;
            }

            string command = args[0];
            int messageCount = 1;
            if (args.Length == 2)
            {
                messageCount = int.Parse(args[1]);
            }

            switch (command)
            {
                case "sendservicebus":
                    await TestBatch(new ServiceBusService(), messageCount);
                    break;
                case "sendstoragequeue":
                    await TestBatch(new StorageQueueService(), messageCount);
                    break;
                default:
                    Console.WriteLine($"Invalid command.");
                    return 1;
            };
            return 0;
        }

        private static async Task TestBatch(IMessageQueueService queueService, int messageCount)
        {
            TestResultCollection results = new();
            for (int i = 0; i < DEFAULT_TEST_TIMES; i++)
            {
                results.TestResults.Add(await queueService.SendMessages(messageCount));
            }
            Console.WriteLine(results.PrintTestResults($"Send {messageCount} messages to {queueService.GetType()} for {DEFAULT_TEST_TIMES}"));
        }
    }
}
