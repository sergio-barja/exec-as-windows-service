namespace Eaws.WindowsService.ProcessRunner
{
    internal class ProcessRunnerServiceConfiguration
    {
        public string ProcessPath { get; set; }

        public string ProcessArguments { get; set; }

        public int WaitTimeUntilRelaunchMs { get; set; }
    }
}