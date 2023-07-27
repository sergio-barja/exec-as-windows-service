using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Eaws.WindowsService
{
    internal class ProgramArguments
    {
        private const string ProcessPathArg = "processPath";
        private const string ProcessArgumentsArg = "processArgs";
        private const string WaitTimeUntilRelaunchMsArg = "minWaitTime";

        private const int DefaultWaitTimeUntilRelaunchMs = 30000;

        public string? ProcessPath { get; private set; }

        public string? ProcessArguments { get; private set; }

        public int WaitTimeUntilRelaunchMs { get; private set; } = DefaultWaitTimeUntilRelaunchMs;

        public bool IsValid => ProcessPath != null;

        public static string HelpText => $"Usage: {Process.GetCurrentProcess().ProcessName}" +
            $" --{ProcessPathArg}=<Path to the process to run>" +
            $" [--{ProcessArgumentsArg}=<process arguments>]" +
            $" [--{WaitTimeUntilRelaunchMsArg}=<valid int for min time between process restarts>]";

        public ProgramArguments(string[] args)
        {
            ParseArguments(args);
        }

        private void ParseArguments(string[] args)
        {
            Regex argumentRegex = new Regex("--(?<name>\\w+)=(?<value>.*)");
            var argumentsByName = args
                    .Select(arg => argumentRegex.Match(arg))
                    .Where(match => match.Success)
                    .ToDictionary(match => match.Groups["name"].Value, match => match.Groups["value"].Value);

            argumentsByName.TryGetValue(ProcessPathArg, out string? processPath);
            ProcessPath = processPath;

            argumentsByName.TryGetValue(ProcessArgumentsArg, out string? processArguments);
            ProcessArguments = processArguments;

            if (argumentsByName.TryGetValue(WaitTimeUntilRelaunchMsArg, out string? waitTimeUntilRelaunch))
                WaitTimeUntilRelaunchMs = int.Parse(waitTimeUntilRelaunch);

        }
    }
}