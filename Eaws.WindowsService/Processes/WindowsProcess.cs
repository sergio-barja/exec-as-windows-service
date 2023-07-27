using System.Diagnostics;

namespace Eaws.WindowsService.Processes
{
    internal class WindowsProcess : IProcessInfo
    {
        private const int ProcessKillWaitTimeMs = 20000;

        private readonly Process process;
        private bool disposedValue;

        public bool HasExited => process.HasExited;

        public DateTimeOffset LaunchedAt => process.StartTime;

        public WindowsProcess(Process process)
        {
            this.process = process;
        }

        public Task WaitForExit(CancellationToken cancellationToken)
        {
            return process.WaitForExitAsync(cancellationToken);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
                return;

            if (disposing && process != null)
            {
                if (!process.HasExited)
                {
                    process.Kill();
                    process.WaitForExit(ProcessKillWaitTimeMs);
                }
                process.Dispose();
            }
            disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
