using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Rinsen.DatabaseInstaller
{
    public class DatabaseScriptRunner
    {
        internal async Task RunAsync(IEnumerable<string> commands, SqlConnection connection, SqlTransaction transaction)
        {
            foreach (var command in commands)
            {
                using (var sqlCommand = new SqlCommand(command, connection, transaction))
                {
                    try
                    {
                        await sqlCommand.ExecuteNonQueryAsync();
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
