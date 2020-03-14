using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Rinsen.DatabaseInstaller
{
    public class Installer
    {
        private readonly DatabaseVersionInstaller _databaseVersionInstaller;
        private readonly VersionHandler _versionHandler;
        private readonly InstallerOptions _installerOptions;
        private readonly ILogger<Installer> _log;
        private readonly IVersionStorage _versionStorage;

        public Installer(DatabaseVersionInstaller databaseVersionInstaller, VersionHandler versionHandler, IVersionStorage versionStorage, InstallerOptions installerOptions, ILogger<Installer> log)
        {
            _databaseVersionInstaller = databaseVersionInstaller;
            _versionHandler = versionHandler;
            _versionStorage = versionStorage;
            _installerOptions = installerOptions;
            _log = log;
        }
        
        public async Task RunAsync(List<DatabaseVersion> databaseVersions)
        {
            using (var connection = new SqlConnection(_installerOptions.ConnectionString))
            {
                _log.LogDebug("DatabaseInstaller started");
                await connection.OpenAsync();
                if (!await _versionStorage.IsInstalled(connection))
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        _log.LogDebug("First installation, installing base version");
                        var installerBaseVersion = new InstallerBaseVersion(_installerOptions.InstalledVersionsDatabaseTableName);
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

        public async Task<IEnumerable<InstallationNameAndVersion>> GetVersionInformation()
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