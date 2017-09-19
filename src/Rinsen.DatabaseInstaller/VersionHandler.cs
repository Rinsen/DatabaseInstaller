using System;
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
            if (!_versionStorage.IsInstalled(connection, transaction))
            {
                throw new InvalidOperationException("Installer is not installed");
            }
            
            InstallationNameAndVersion installedNameAndVersion = _versionStorage.Get(name, connection, transaction);

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

        internal virtual IEnumerable<InstallationNameAndVersion> GetInstalledVersionsInformation(SqlConnection connection, SqlTransaction transaction)
        {
            if (_versionStorage.IsInstalled(connection, transaction))
            {
                return _versionStorage.GetAll(connection, transaction);
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

        internal InstallVersionScope BeginInstallVersionScope(DatabaseVersion version, SqlConnection connection, SqlTransaction transaction)
        {
            // Verify that this version really should be installed
            var installedVersion = GetCurrentInstalledVersionAndValidatePreInstallationState(version, connection, transaction);

            // Update information that this installation is beginning to install now, make sure that no one is in between, throw is someone is
            int result = _versionStorage.StartInstallation(installedVersion, connection, transaction);

            if (result != 1)
            {
                throw new InvalidOperationException(string.Format("Starting installation for version {0} for {1} failed with count {2}", version.Version, version.InstallationName, result));
            }

            return new InstallVersionScope(_versionStorage, version, connection, transaction);
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

        
    }
}