namespace MessageQueuePerformanceTestsConsole.Models
{
    public class TestResult
    {
        public int TotalMessageCount { get; set; }

        public TimeSpan SetupClientDuration { get; set; }

        public TimeSpan FirstMessageDuration { get; set; }

        public TimeSpan Duration { get; set; }
    }
}
