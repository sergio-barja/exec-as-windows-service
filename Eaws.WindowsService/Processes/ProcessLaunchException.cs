using System.Runtime.Serialization;

namespace Eaws.WindowsService.Processes
{
    public class ProcessLaunchException : ApplicationException
    {
        public ProcessLaunchException() : base()
        {
        }

        public ProcessLaunchException(string processPath, string processArguments, Exception? innerException = null) : base($"Process {processPath} {processArguments} cannot be launched", innerException)
        {

        }

        protected ProcessLaunchException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
