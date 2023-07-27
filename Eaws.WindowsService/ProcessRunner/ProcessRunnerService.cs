using Eaws.WindowsService.Processes;
using Meziantou.Framework.Win32;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace Eaws.WindowsService.ProcessRunner
{
    [SupportedOSPlatform("windows5.1.2600")]
    internal class ProcessRunnerService : BackgroundService
    {
        private readonly ILogger<ProcessRunnerService> logger;
        private readonly IOptions<ProcessRunnerServiceConfiguration> config;
        private readonly IProcessLauncher processLauncher;
        private IProcessInfo? process;
        private JobObject jobObject;

        public ProcessRunnerService(
            ILogger<ProcessRunnerService> logger,
            IOptions<ProcessRunnerServiceConfiguration> config,
            IProcessLauncher processLauncher)
        {
            this.logger = logger;
            this.config = config;
            this.processLauncher = processLauncher;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            jobObject = InitializeJobObject();
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Starting process {processpath} {processargs}", config.Value.ProcessPath, config.Value.ProcessArguments);

                process = processLauncher.Launch(config.Value.ProcessPath, config.Value.ProcessArguments);
                await process!.WaitForExit(stoppingToken);

                if (!stoppingToken.IsCancellationRequested)
                {
                    await WaitUntilRelaunch();
                }
            }
            EnsureProcessIsClosed();
        }

        private static JobObject InitializeJobObject()
        {
            var job = new JobObject();
            job.SetLimits(new JobObjectLimits()
            {
                Flags = JobObjectLimitFlags.DieOnUnhandledException |
                        JobObjectLimitFlags.KillOnJobClose,
            });
            job.AssignProcess(Process.GetCurrentProcess());
            return job;
        }

        private Task WaitUntilRelaunch()
        {
            if (process == null)
                return Task.CompletedTask;

            var ellapsedTimeSinceLaunch = DateTimeOffset.Now.Subtract(process.LaunchedAt);
            var minTimeToWait = TimeSpan.FromMilliseconds(config.Value.WaitTimeUntilRelaunchMs);

            if (minTimeToWait <= ellapsedTimeSinceLaunch)
                return Task.CompletedTask;

            return Task.Delay(minTimeToWait.Subtract(ellapsedTimeSinceLaunch));
        }

        private void EnsureProcessIsClosed()
        {
            if (process != null)
            {
                process.Dispose();
                process = null;
            }
            if (jobObject != null)
            {
                jobObject.Dispose();
                jobObject = null;
            }
        }

        ~ProcessRunnerService()
        {
            EnsureProcessIsClosed();
        }
    }
}