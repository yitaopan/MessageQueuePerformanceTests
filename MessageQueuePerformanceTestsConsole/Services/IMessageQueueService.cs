using MessageQueuePerformanceTestsConsole.Models;

namespace MessageQueuePerformanceTestsConsole.Services
{
    internal interface IMessageQueueService
    {
        Task<TestResult> SendMessages(int messageCount);

        Task<string> DiscardAllMessages();
    }
}
