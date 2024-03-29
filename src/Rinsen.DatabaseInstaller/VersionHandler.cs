﻿using System;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rinsen.DatabaseInstaller
{
    public class VersionHandler
    {
        readonly IVersionStorage _versionStorage;

        public VersionHandler(IVersionStorage versionStorage)
        {
            _versionStorage = versionStorage;
        }

        internal async Task<InstallationNameAndVersion> GetInstalledVersionAsync(string name, SqlConnection connection, SqlTransaction transaction)
        {
            if (!await _versionStorage.IsInstalledAsync(connection, transaction))
            {
                throw new InvalidOperationException("Installer is not installed");
            }
            
            InstallationNameAndVersion installedNameAndVersion = await _versionStorage.GetAsync(name, connection, transaction);

            if (installedNameAndVersion == default(InstallationNameAndVersion))
            {
                installedNameAndVersion = new InstallationNameAndVersion
                {
                    InstallationName = name,
                    InstalledVersion = 0,
                    PreviousVersion = 0,
                    StartedInstallingVersion = 0
                };

                await _versionStorage.CreateAsync(installedNameAndVersion, connection, transaction);
            }

            return installedNameAndVersion;
        }

        internal async Task<IEnumerable<InstallationNameAndVersion>> GetInstalledVersionsInformation(SqlConnection connection, SqlTransaction transaction)
        {
            if (await _versionStorage.IsInstalledAsync(connection, transaction))
            {
                return await _versionStorage.GetAllAsync(connection, transaction);
            }
            else
            {
                return Enumerable.Empty<InstallationNameAndVersion>();
            }
        }

        internal async Task InstallBaseVersion(InstallerBaseVersion installerBaseVersion, SqlConnection connection, SqlTransaction transaction)
        {
            var installedNameAndVersion = new InstallationNameAndVersion
            {
                InstallationName = installerBaseVersion.InstallationName,
                InstalledVersion = installerBaseVersion.Version,
                PreviousVersion = 0,
                StartedInstallingVersion = installerBaseVersion.Version
            };

            await _versionStorage.CreateAsync(installedNameAndVersion, connection, transaction);
        }

        internal async Task<InstallVersionScope> BeginInstallVersionScopeAsync(DatabaseVersion version, SqlConnection connection, SqlTransaction transaction)
        {
            // Verify that this version really should be installed
            var installedVersion = await GetCurrentInstalledVersionAndValidatePreInstallationStateAsync(version, connection, transaction);

            // Update information that this installation is beginning to install now, make sure that no one is in between, throw is someone is
            int result = await _versionStorage.StartInstallationAsync(installedVersion, connection, transaction);

            if (result != 1)
            {
                throw new InvalidOperationException(string.Format("Starting installation for version {0} for {1} failed with count {2}", version.Version, version.InstallationName, result));
            }

            return new InstallVersionScope(_versionStorage, version, connection, transaction);
        }

        private async Task<InstallationNameAndVersion> GetCurrentInstalledVersionAndValidatePreInstallationStateAsync(DatabaseVersion version, SqlConnection connection, SqlTransaction transaction)
        {
            // Get installation row from database
            var installedVersion = await GetInstalledVersionAsync(version.InstallationName, connection, transaction);

            if (installedVersion.InstalledVersion >= version.Version)
            {
                throw new InvalidOperationException(string.Format("Version ({0}) is already installed for {1}", version.Version, version.InstallationName));
            }
            if (installedVersion.StartedInstallingVersion >= version.Version)
            {
                throw new InvalidOperationException(string.Format("Version ({0}) installation is already in progress for {1}", version.Version, version.InstallationName));
            }
            if (installedVersion.StartedInstallingVersion != version.Version - 1)
            {
                throw new InvalidOperationException(string.Format("Unknown version missmatch in version ({0}) for {1}", version.Version, version.InstallationName));
            }

            return installedVersion;
        }

        
    }
}
