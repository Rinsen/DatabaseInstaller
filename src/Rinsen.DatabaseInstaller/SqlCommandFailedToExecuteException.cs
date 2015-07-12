using System;

namespace Rinsen.DatabaseInstaller
{
    public class SqlCommandFailedToExecuteException : Exception
    {
        public SqlCommandFailedToExecuteException(string message, Exception innerException)
            : base (message, innerException)
        {

        }
    }
}
