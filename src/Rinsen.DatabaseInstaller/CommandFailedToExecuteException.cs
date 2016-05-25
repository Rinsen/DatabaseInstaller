using System;

namespace Rinsen.DatabaseInstaller
{
    public class CommandFailedToExecuteException : Exception
    {
        public CommandFailedToExecuteException(string message, Exception innerException)
            : base (message, innerException)
        {

        }
    }
}
