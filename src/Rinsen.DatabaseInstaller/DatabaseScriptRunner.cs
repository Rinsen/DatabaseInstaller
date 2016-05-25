using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Rinsen.DatabaseInstaller
{
    public class DatabaseScriptRunner
    {
        internal void Run(List<string> commands, SqlConnection connection, SqlTransaction transaction)
        {
            foreach (var command in commands)
            {
                using (var sqlCommand = new SqlCommand(command, connection, transaction))
                {
                    try
                    {
                        sqlCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        var exception = new CommandFailedToExecuteException(string.Format("Installation error for command {0}", command), ex);
                        throw exception;
                    }
                }
            }
        }
    }
}
