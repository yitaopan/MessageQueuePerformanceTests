namespace MessageQueuePerformanceTestsConsole.Models
{
    public class TestResultCollection
    {
        public List<TestResult> TestResults { get; set; } = [];

        public string PrintTestResults(string testName)
        {
            string result = $"Result for {testName}:\n" +
                $"{"TotalMessageCount", -30}" +
                $"{"SetupClientDuration (ms)", -30}" +
                $"{"FirstMessageDuration (ms)", -30}" +
                $"{"Duration (ms)", -30}\n";

            double averageSetupClientDurations = 0;
            double averageFirstMessageDurations = 0;
            double averageDurations = 0;
            foreach (TestResult testResult in TestResults)
            {
                averageSetupClientDurations += testResult.SetupClientDuration.TotalMilliseconds;
                averageFirstMessageDurations += testResult.FirstMessageDuration.TotalMilliseconds;
                averageDurations += testResult.Duration.TotalMilliseconds;

                result += $"{testResult.TotalMessageCount, -30}" +
                    $"{testResult.SetupClientDuration.TotalMilliseconds,-30}" +
                    $"{testResult.FirstMessageDuration.TotalMilliseconds,-30}" +
                    $"{testResult.Duration.TotalMilliseconds,-30}\n";
            }
            result += $"{TestResults[0].TotalMessageCount,-30}" +
                $"{averageSetupClientDurations / TestResults.Count,-30}" +
                $"{averageFirstMessageDurations / TestResults.Count,-30}" +
                $"{averageDurations / TestResults.Count,-30} in average\n";
            return result;
        }
    }
}
