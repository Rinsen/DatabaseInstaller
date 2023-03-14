using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Rinsen.DatabaseInstaller
{
    internal class Installer
    {
        private readonly DatabaseVersionInstaller _databaseVersionInstaller;
        private readonly DatabaseInitializer _databaseInitializer;
        private readonly VersionHandler _versionHandler;
        private readonly ILogger<Installer> _log;
        private readonly InstallerOptions _installerOptions;

        public Installer(DatabaseVersionInstaller databaseVersionInstaller,
            DatabaseInitializer databaseInitializer,
            VersionHandler versionHandler,
            InstallerOptions installerOptions,
            ILogger<Installer> log)
        {
            _databaseVersionInstaller = databaseVersionInstaller;
            _databaseInitializer = databaseInitializer;
            _versionHandler = versionHandler;
            _installerOptions = installerOptions;
            _log = log;
        }
        
        public async Task RunAsync(List<DatabaseVersion> databaseVersions)
        {
            using var connection = new SqlConnection(_installerOptions.ConnectionString);

            var fail = 1;
            while (fail < 51)
            {
                try
                {
                    await connection.OpenAsync();
                }
                catch (Exception e)
                {
                    _log.LogInformation("Failed to connect to database");
                    fail++;

                    if (fail == 50)
                    {
                        throw new Exception("Failed to connect to SQL Server", e);
                    }

                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            }
            
            _log.LogInformation("DatabaseInstaller started");

            await _databaseInitializer.InitializeAsync(connection);

            using var transaction = connection.BeginTransaction();

            await _databaseVersionInstaller.InstallAsync(databaseVersions, connection, transaction);
            
            _log.LogDebug("DatabaseInstaller finished, commit changes");

            transaction.Commit();
            _log.LogDebug("Commit completed");
        }

        public void RollbackVersions(string name, int toVersion)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<InstallationNameAndVersion>> GetVersionInformationAsync()
        {
            using var connection = new SqlConnection(_installerOptions.ConnectionString);
            
            await connection.OpenAsync();
            
            using var transaction = connection.BeginTransaction();
            
            return await _versionHandler.GetInstalledVersionsInformation(connection, transaction);
        } 
    }
}
