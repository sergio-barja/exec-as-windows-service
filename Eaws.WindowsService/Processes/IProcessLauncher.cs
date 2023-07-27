namespace Eaws.WindowsService.Processes
{
    internal interface IProcessLauncher
    {
        IProcessInfo Launch(string processPath, string processArguments);
    }
}