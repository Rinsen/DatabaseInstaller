using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
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

        public Installer(DatabaseVersionInstaller changeInstaller, VersionHandler versionHandler, IVersionStorage versionStorage, InstallerOptions installerOptions, ILogger<Installer> log)
        {
            _databaseVersionInstaller = changeInstaller;
            _versionHandler = versionHandler;
            _versionStorage = versionStorage;
            _installerOptions = installerOptions;
            _log = log;
        }
        
        public void Run(IEnumerable<DatabaseVersion> databaseVersions)
        {
            using (var connection = new SqlConnection(_installerOptions.ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {

                    _log.LogDebug("DatabaseInstaller started");
                    if (!_versionStorage.IsInstalled(connection, transaction))
                    {
                        _log.LogDebug("First installation, installing base version");
                        var installerBaseVersion = new InstallerBaseVersion(_installerOptions.InstalledVersionsDatabaseTableName);
                        _databaseVersionInstaller.InstallBaseVersion( installerBaseVersion, connection, transaction);
                    }

                    _databaseVersionInstaller.Install(databaseVersions.ToList(), connection, transaction);
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

        public IEnumerable<InstallationNameAndVersion> GetVersionInformation()
        {
            using (var connection = new SqlConnection(_installerOptions.ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    return _versionHandler.GetInstalledVersionsInformation(connection, transaction);
                }
            }
        } 
    }
}