﻿using System;
using System.Data.SqlClient;
using System.Linq;
using System.Collections.Generic;

namespace Rinsen.DatabaseInstaller
{
    public class VersionHandler
    {
        readonly IVersionStorage _versionStorage;

        public VersionHandler(InstallerOptions installerOptions, IVersionStorage versionStorage)
        {
            _versionStorage = versionStorage;
        }

        internal virtual InstallationNameAndVersion GetInstalledVersion(string name, SqlConnection connection, SqlTransaction transaction)
        {
            if (!IsInstalled())
            {
                throw new InvalidOperationException("Installer is not installed");
            }
            
            InstallationNameAndVersion installedNameAndVersion = _versionStorage.Get(name, connection);

            if (installedNameAndVersion == default(InstallationNameAndVersion))
            {
                installedNameAndVersion = new InstallationNameAndVersion
                {
                    InstallationName = name,
                    InstalledVersion = 0,
                    PreviousVersion = 0,
                    StartedInstallingVersion = 0
                };

                _versionStorage.Create(installedNameAndVersion, connection, transaction);
            }

            return installedNameAndVersion;
        }

        internal virtual IEnumerable<InstallationNameAndVersion> GetInstalledVersionsInformation(SqlConnection connection)
        {
            if (IsInstalled())
            {
                return _versionStorage.GetAll(connection);
            }
            else
            {
                return Enumerable.Empty<InstallationNameAndVersion>();
            }
        }

        internal void InstallBaseVersion(InstallerBaseVersion installerBaseVersion, SqlConnection connection, SqlTransaction transaction)
        {
            var installedNameAndVersion = new InstallationNameAndVersion
            {
                InstallationName = installerBaseVersion.InstallationName,
                InstalledVersion = installerBaseVersion.Version,
                PreviousVersion = 0,
                StartedInstallingVersion = installerBaseVersion.Version
            };

            _versionStorage.Create(installedNameAndVersion, connection, transaction);
        }

        internal bool IsInstalled()
        {
            return _versionStorage.IsInstalled();
        }

        internal void BeginInstallVersion(DatabaseVersion version, SqlConnection connection, SqlTransaction transaction)
        {
            // Verify that this version really should be installed
            var installedVersion = GetCurrentInstalledVersionAndValidatePreInstallationState(version, connection, transaction);

            // Update information that this installation is beginning to install now, make sure that no one is in between, throw is someone is
            int result = _versionStorage.StartInstallation(installedVersion, connection, transaction);

            if (result != 1)
            {
                throw new InvalidOperationException(string.Format("Starting installation for version {0} for {1} failed with count {2}", version.Version, version.InstallationName, result));
            }
        }

        internal void SetVersionInstalled(DatabaseVersion version, SqlConnection connection, SqlTransaction transaction)
        {
            InstallationNameAndVersion installedVersion = GetCurrentInstalledVersionAndValidatePostInstallationState(version, connection, transaction);

            // Update information that this installation is ended now, make sure that no one is in between, throw is someone is
            int result = _versionStorage.EndInstallation(installedVersion, connection, transaction);

            if (result != 1)
            {
                throw new InvalidOperationException(string.Format("Starting installation for version {0} for {1} failed with count {2}", version.Version, version.InstallationName, result));
            }
        }

        private InstallationNameAndVersion GetCurrentInstalledVersionAndValidatePreInstallationState(DatabaseVersion version, SqlConnection connection, SqlTransaction transaction)
        {
            // Get installation row from database
            var installedVersion = GetInstalledVersion(version.InstallationName, connection, transaction);

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

        private InstallationNameAndVersion GetCurrentInstalledVersionAndValidatePostInstallationState(DatabaseVersion version, SqlConnection connection, SqlTransaction transaction)
        {
            // Get installation row from database
            var installedVersion = GetInstalledVersion(version.InstallationName, connection, transaction);

            // Verify that this version really should have been installed now
            if (installedVersion.InstalledVersion >= version.Version)
            {
                throw new InvalidOperationException(string.Format("Version ({0}) is already installed for {1}", version.Version, version.InstallationName));
            }
            if (installedVersion.StartedInstallingVersion != version.Version)
            {
                throw new InvalidOperationException(string.Format("Version ({0}) is not in progress for {1} as it should be", version.Version, version.InstallationName));
            }

            return installedVersion;
        }
    }
}