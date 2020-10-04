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

        internal async Task<int> RunAsync(string command, SqlConnection connection, string parameterName, string parameterValue)
        {
            using (var sqlCommand = new SqlCommand(command, connection))
            {
                sqlCommand.Parameters.AddWithValue(parameterName, parameterValue);

                try
                {
                    using (var reader = await sqlCommand.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            await reader.ReadAsync();

                            return (int)reader[0];
                        }

                        throw new Exception("Unknown fail when executing command");
                    }
                }
                catch (Exception ex)
                {
                    var exception = new CommandFailedToExecuteException(string.Format("Installation error for command {0} with parameter name {1} with value {2}", command, parameterName, parameterValue), ex);

                    throw exception;
                }
            }
        }
    }
}
