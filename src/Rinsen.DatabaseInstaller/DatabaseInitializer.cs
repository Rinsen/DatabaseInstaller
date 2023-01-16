using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Rinsen.DatabaseInstaller
{
    public class DatabaseInitializer
    {

        private readonly IVersionStorage _versionStorage;
        private readonly DatabaseScriptRunner _databaseScriptRunner;
        private readonly DatabaseVersionInstaller _databaseVersionInstaller;
        private readonly InstallerOptions _installerOptions;
        private readonly ILogger<DatabaseInitializer> _logger;

        public DatabaseInitializer(IVersionStorage versionStorage,
            DatabaseScriptRunner databaseScriptRunner,
            DatabaseVersionInstaller databaseVersionInstaller,
            InstallerOptions installerOptions,
            ILogger<DatabaseInitializer> logger)
        {
            _versionStorage = versionStorage;
            _databaseScriptRunner = databaseScriptRunner;
            _logger = logger;
            _databaseVersionInstaller = databaseVersionInstaller;
            _installerOptions = installerOptions;
        }

        internal async Task Initialize(SqlConnection connection)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"IF '{_installerOptions.DatabaseName}' NOT IN (SELECT [name] FROM [master].[sys].[databases] WHERE [name] NOT IN ('master', 'tempdb', 'model', 'msdb'))");
            sb.AppendLine("BEGIN");
            sb.AppendLine($"    CREATE DATABASE {_installerOptions.DatabaseName};");
            sb.AppendLine("    SELECT 1;");
            sb.AppendLine("END");
            sb.AppendLine("ELSE");
            sb.AppendLine("BEGIN");
            sb.AppendLine("    SELECT 0;");
            sb.AppendLine("END");

            var status = await _databaseScriptRunner.RunAsync(sb.ToString(), connection, "@databaseName", _installerOptions.DatabaseName);

            if (status == 1)
            {
                _logger.LogInformation($"Created database {_installerOptions.DatabaseName}");
            }
            else
            {
                _logger.LogInformation($"Database exists {_installerOptions.DatabaseName}");
            }

            if (!await _versionStorage.IsInstalled(connection))
            {
                using (var transaction = connection.BeginTransaction())
                {
                    _logger.LogDebug("First installation, installing base version");
                    var installerBaseVersion = new InstallerBaseVersion();
                    await _databaseVersionInstaller.InstallBaseVersion(installerBaseVersion, connection, transaction);
                    transaction.Commit();
                    _logger.LogDebug("Commit completed");
                }
            }
            else
            {
                _logger.LogDebug("Database already have base version installed");
            }
        }
    }
}
