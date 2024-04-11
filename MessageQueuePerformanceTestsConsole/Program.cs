using MessageQueuePerformanceTestsConsole.Services;

namespace MessageQueuePerformanceTestsConsole
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            //await Task.Delay(1);
            //if (args.Length == 0)
            //{
            //    Console.WriteLine($"Please provide commands.");
            //    return 1;
            //}
            //Console.WriteLine($"Arg: {args[0]}");

            string result = await ServiceBusService.SendServiceBusMessages(1);
            Console.WriteLine(result);
            return 0;
        }
    }
}
