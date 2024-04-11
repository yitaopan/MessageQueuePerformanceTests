using MessageQueuePerformanceTestsConsole.Services;

namespace MessageQueuePerformanceTestsConsole
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine($"Please provide commands.");
                return 1;
            }
            string command = args[0];
            switch (command)
            {
                case "sendservicebus":
                    int sbMessageCount = args[1] != null ? int.Parse(args[1]) : 1;
                    string sbResult = await ServiceBusService.SendServiceBusMessages(sbMessageCount);
                    Console.WriteLine(sbResult);
                    break;

                case "sendstoragequeue":
                    int sqMessageCount = args[1] != null ? int.Parse(args[1]) : 1;
                    string sqResult = await StorageQueueService.SendStorageQueueMessages(sqMessageCount);
                    Console.WriteLine(sqResult);
                    break;

                default:
                    Console.WriteLine($"Invalid command.");
                    return 1;
            };
            return 0;
        }
    }
}
