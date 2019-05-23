using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Rinsen.DatabaseInstaller
{
    public class DatabaseVersionInstaller
    {
        private readonly VersionHandler _versionHandler;
        private readonly DatabaseScriptRunner _databaseScriptRunner;
        private readonly ILogger<DatabaseVersionInstaller> _logger;

        public DatabaseVersionInstaller(VersionHandler versionHandler, DatabaseScriptRunner databaseScriptRunner, ILogger<DatabaseVersionInstaller> logger)
        {
            _versionHandler = versionHandler;
            _databaseScriptRunner = databaseScriptRunner;
            _logger = logger;
        }

        internal void Install(List<DatabaseVersion> databaseVersions, SqlConnection connection, SqlTransaction transaction)
        {
            foreach (var installationName in databaseVersions.Select(v => v.InstallationName).Distinct())
            {
                var versions = databaseVersions.Where(m => m.InstallationName == installationName).ToList();

                if (versions.GroupBy(m => m.Version).Where(c => c.Count() > 1).Any())
                {
                    throw new ArgumentException(string.Format("There can only be one unique version {0} for installation name {1}", databaseVersions.GroupBy(m => m.Version).Where(c => c.Count() > 1).First(), installationName));
                }

                var installedVersion = _versionHandler.GetInstalledVersion(installationName, connection, transaction);

                if (versions.Where(m => m.Version > installedVersion.InstalledVersion).Any())
                {
                    InstallVersionsForSingleInstallationName(versions.Where(m => m.Version > installedVersion.InstalledVersion).OrderBy(m => m.Version), connection, transaction);
                }
            }
        }

        internal void InstallBaseVersion(InstallerBaseVersion installerBaseVersion, SqlConnection connection, SqlTransaction transaction)
        {
            _databaseScriptRunner.Run(installerBaseVersion.UpCommands, connection, transaction);

            _versionHandler.InstallBaseVersion(installerBaseVersion, connection, transaction);
        }

        private void InstallVersionsForSingleInstallationName(IOrderedEnumerable<DatabaseVersion> orderedVersionsForInstallationName, SqlConnection connection, SqlTransaction transaction)
        {
            foreach (var version in orderedVersionsForInstallationName)
            {
                using (var scope = _versionHandler.BeginInstallVersionScope(version, connection, transaction))
                {
                    try
                    {
                        _databaseScriptRunner.Run(version.UpCommands, connection, transaction);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Failed to run script in {installationName} for version {version}", version.InstallationName, version.Version);
                    }
                }
            }
        }
    }
}