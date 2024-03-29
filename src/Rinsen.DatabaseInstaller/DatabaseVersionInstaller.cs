﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;


namespace Rinsen.DatabaseInstaller
{
    public class DatabaseVersionInstaller
    {
        private readonly VersionHandler _versionHandler;
        private readonly DatabaseScriptRunner _databaseScriptRunner;
        private readonly InstallerOptions _installerOptions;
        private readonly ILogger<DatabaseVersionInstaller> _logger;

        public DatabaseVersionInstaller(VersionHandler versionHandler,
            DatabaseScriptRunner databaseScriptRunner,
            InstallerOptions installerOptions,
            ILogger<DatabaseVersionInstaller> logger)
        {
            _versionHandler = versionHandler;
            _databaseScriptRunner = databaseScriptRunner;
            _installerOptions = installerOptions;
            _logger = logger;
        }

        internal async Task InstallAsync(List<DatabaseVersion> databaseVersions, SqlConnection connection, SqlTransaction transaction)
        {
            foreach (var installationName in databaseVersions.Select(v => v.InstallationName).Distinct())
            {
                var versions = databaseVersions.Where(m => m.InstallationName == installationName).ToList();

                if (versions.GroupBy(m => m.Version).Where(c => c.Count() > 1).Any())
                {
                    throw new ArgumentException(string.Format("There can only be one unique version {0} for installation name {1}", databaseVersions.GroupBy(m => m.Version).Where(c => c.Count() > 1).First(), installationName));
                }

                var installedVersion = await _versionHandler.GetInstalledVersionAsync(installationName, connection, transaction);

                if (versions.Where(m => m.Version > installedVersion.InstalledVersion).Any())
                {
                    await InstallVersionsForSingleInstallationNameAsync(versions.Where(m => m.Version > installedVersion.InstalledVersion).OrderBy(m => m.Version), connection, transaction);
                }
            }
        }

        internal async Task InstallBaseVersionAsync(InstallerBaseVersion installerBaseVersion, SqlConnection connection, SqlTransaction transaction)
        {
            await _databaseScriptRunner.RunAsync(installerBaseVersion.GetUpCommands(_installerOptions), connection, transaction);

            await _versionHandler.InstallBaseVersion(installerBaseVersion, connection, transaction);
        }

        private async Task InstallVersionsForSingleInstallationNameAsync(IOrderedEnumerable<DatabaseVersion> orderedVersionsForInstallationName, SqlConnection connection, SqlTransaction transaction)
        {
            foreach (var version in orderedVersionsForInstallationName)
            {
                await using var scope = await _versionHandler.BeginInstallVersionScopeAsync(version, connection, transaction);

                try
                {
                    await _databaseScriptRunner.RunAsync(version.GetUpCommands(_installerOptions), connection, transaction);

                    scope.Complete();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to run script in {installationName} for version {version}", version.InstallationName, version.Version);

                    scope.Fail(e);
                }
            }
        }
    }
}
