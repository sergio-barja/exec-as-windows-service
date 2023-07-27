using System.Diagnostics;

namespace Eaws.WindowsService.Processes
{

    internal class ProcessLauncher : IProcessLauncher
    {
        public IProcessInfo Launch(string processPath, string processArguments)
        {
            var process = Process.Start(BuildProcessStartInfo(processPath, processArguments));
            if (process == null)
                throw new ProcessLaunchException(processPath, processArguments);
            return new WindowsProcess(process);
        }

        private ProcessStartInfo BuildProcessStartInfo(string processPath, string processArguments)
        {
            return new ProcessStartInfo(processPath, processArguments)
            {
                CreateNoWindow = true,
                WorkingDirectory = Path.GetDirectoryName(processPath)
            };
        }
    }
}
