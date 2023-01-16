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
            using (var connection = new SqlConnection(_installerOptions.ConnectionString))
            {
                await connection.OpenAsync();
                _log.LogDebug("DatabaseInstaller started");

                await _databaseInitializer.Initialize(connection);
                
                using (var transaction = connection.BeginTransaction())
                {
                    await _databaseVersionInstaller.Install(databaseVersions, connection, transaction);
                    _log.LogDebug("DatabaseInstaller finished, commit changes");
                    transaction.Commit();
                    _log.LogDebug("Commit completed");
                }
            }
        }

        public void RollbackVersions(string name, int toVersion)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<InstallationNameAndVersion>> GetVersionInformationAsync()
        {
            using (var connection = new SqlConnection(_installerOptions.ConnectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    return await _versionHandler.GetInstalledVersionsInformation(connection, transaction);
                }
            }
        } 
    }
}
