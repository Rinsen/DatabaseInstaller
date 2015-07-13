using Rinsen.DatabaseInstaller.Sql;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rinsen.DatabaseInstaller
{
    public class DatabaseVersionInstaller
    {
        private readonly InstallerOptions _installerOptions;
        private readonly VersionHandler _versionHandler;
        private readonly DatabaseScriptRunner _databaseScriptRunner;

        public DatabaseVersionInstaller(InstallerOptions installerOptions, VersionHandler versionHandler, DatabaseScriptRunner databaseScriptRunner)
        {
            _installerOptions = installerOptions;
            _versionHandler = versionHandler;
            _databaseScriptRunner = databaseScriptRunner;
        }

        internal void Install(List<DatabaseVersion> databaseVersions)
        {
            foreach (var installationName in databaseVersions.Select(v => v.InstallationName).Distinct())
            {
                var versions = databaseVersions.Where(m => m.InstallationName == installationName).ToList();

                if (versions.GroupBy(m => m.Version).Where(c => c.Count() > 1).Any())
                {
                    throw new ArgumentException(string.Format("There can only be one unique version {0} for installation name {1}", databaseVersions.GroupBy(m => m.Version).Where(c => c.Count() > 1).First(), installationName));
                }

                var installedVersion = _versionHandler.GetInstalledVersion(installationName);

                if (!versions.Where(m => m.Version > installedVersion.InstalledVersion).Any())
                {
                    return;
                }

                InstallVersionsForSingleInstallationName(versions.Where(m => m.Version > installedVersion.InstalledVersion).OrderBy(m => m.Version));
            }
        }

        internal void InstallBaseVersion(InstallerBaseVersion installerBaseVersion)
        {
            installerBaseVersion.Up();

            _databaseScriptRunner.Run(installerBaseVersion.Commands);

            _versionHandler.InstallBaseVersion(installerBaseVersion);
        }

        private void InstallVersionsForSingleInstallationName(IOrderedEnumerable<DatabaseVersion> orderedVersionsForInstallationName)
        {
            foreach (var version in orderedVersionsForInstallationName)
            {
                _versionHandler.BeginInstallVersion(version);

                var tableCollection = new List<Table>();
                version.AddTables(tableCollection);
                version.SetTables(tableCollection);

                version.PrepareUp();
                try
                {
                    _databaseScriptRunner.Run(version.Commands);
                }
                catch (SqlCommandFailedToExecuteException e)
                {
                    _versionHandler.UndoBeginInstallVersion(version);
                    throw e;
                }
                

                _versionHandler.SetVersionInstalled(version);

                
            }
        }

        
    }
}