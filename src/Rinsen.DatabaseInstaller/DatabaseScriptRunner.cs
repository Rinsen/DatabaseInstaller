using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Rinsen.DatabaseInstaller
{
    public class DatabaseScriptRunner
    {
        private readonly InstallerOptions _installerOptions;

        public DatabaseScriptRunner(InstallerOptions installerOptions)
        {
            _installerOptions = installerOptions;
        }

        internal void Run(List<string> commands)
        {
            var currentCommand = string.Empty;
            using (var connection = new SqlConnection(_installerOptions.ConnectionString))
            {
                connection.Open();

                var sqlCommand = connection.CreateCommand();
                var transaction = connection.BeginTransaction();

                sqlCommand.Transaction = transaction;

                try
                {
                    foreach (var command in commands)
                    {
                        currentCommand = command;
                        sqlCommand.CommandText = currentCommand;
                        sqlCommand.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    var exception = new SqlCommandFailedToExecuteException(string.Format("Installation error for command {0}", currentCommand), ex);
                    try
                    {
                        transaction.Rollback();
                        throw exception;
                    }
                    catch (Exception ex2)
                    {
                        throw new AggregateException("Rollback failed to execute", new List<Exception> { exception, ex2 });
                    }
                }
            }
        }
    }
}
