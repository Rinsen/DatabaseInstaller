using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Rinsen.DatabaseInstaller
{
    internal class Installer
    {
        private readonly DatabaseVersionInstaller _databaseVersionInstaller;
        private readonly DatabaseScriptRunner _databaseScriptRunner;
        private readonly VersionHandler _versionHandler;
        private readonly ILogger<Installer> _log;
        private readonly IVersionStorage _versionStorage;
        private readonly InstallerOptions _installerOptions;

        public Installer(DatabaseVersionInstaller databaseVersionInstaller,
            DatabaseScriptRunner databaseScriptRunner,
            VersionHandler versionHandler,
            IVersionStorage versionStorage,
            InstallerOptions installerOptions,
            ILogger<Installer> log)
        {
            _databaseVersionInstaller = databaseVersionInstaller;
            _databaseScriptRunner = databaseScriptRunner;
            _versionHandler = versionHandler;
            _versionStorage = versionStorage;
            _installerOptions = installerOptions;
            _log = log;
        }
        
        public async Task RunAsync(List<DatabaseVersion> databaseVersions)
        {
            using (var connection = new SqlConnection(_installerOptions.ConnectionString))
            {
                await connection.OpenAsync();
                _log.LogDebug("DatabaseInstaller started");

                foreach (var database in databaseVersions.Where(m => m.Database is object))
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        await _databaseScriptRunner.RunAsync(database.GetUpCommands(_installerOptions), connection, transaction);
                        transaction.Commit();
                    }
                }

                if (!await _versionStorage.IsInstalled(connection))
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        _log.LogDebug("First installation, installing base version");
                        var installerBaseVersion = new InstallerBaseVersion();
                        await _databaseVersionInstaller.InstallBaseVersion(installerBaseVersion, connection, transaction);
                        transaction.Commit();
                        _log.LogDebug("Commit completed");
                    }
                }
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
