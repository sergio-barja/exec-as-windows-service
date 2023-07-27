namespace Eaws.WindowsService.Processes
{
    internal interface IProcessInfo : IDisposable
    {
        bool HasExited { get; }

        Task WaitForExit(CancellationToken cancellationToken);

        DateTimeOffset LaunchedAt { get; }

    }
}
