using Eaws.WindowsService.Processes;
using Eaws.WindowsService.ProcessRunner;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;
using System.Runtime.Versioning;

namespace Eaws.WindowsService
{
    [SupportedOSPlatform("windows5.1.2600")]
    internal class Program
    {
        private static void Main(string[] args)
        {
            ProgramArguments arguments = new ProgramArguments(args);

            if (!arguments.IsValid)
            {
                Console.WriteLine(ProgramArguments.HelpText);
                return;
            }

            IHostBuilder builder = Host.CreateDefaultBuilder(args)
                .UseWindowsService(options =>
                {
                    options.ServiceName = "Exec As Windows Service";
                })
                .ConfigureServices((context, services) =>
                {
                    LoggerProviderOptions.RegisterProviderOptions<EventLogSettings, EventLogLoggerProvider>(services);

                    services
                        .AddOptions<ProcessRunnerServiceConfiguration>()
                        .Configure(options =>
                        {
                            options.ProcessPath = arguments.ProcessPath!;
                            options.ProcessArguments = arguments.ProcessArguments!;
                            options.WaitTimeUntilRelaunchMs = arguments.WaitTimeUntilRelaunchMs;
                        });

                    services
                        .AddHostedService<ProcessRunnerService>()
                        .AddTransient<IProcessLauncher, ProcessLauncher>()
                        .AddLogging(builder =>
                        {
                            builder.AddConfiguration(
                                context.Configuration.GetSection("Logging"));
                        });
                });

            IHost host = builder.Build();

            host.Run();
        }

    }
}