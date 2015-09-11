using System;
using System.Data.SqlClient;
using System.Linq;
using System.Collections.Generic;

namespace Rinsen.DatabaseInstaller
{
    public class VersionHandler
    {
        readonly InstallerOptions _installerOptions;
        readonly IVersionStorage _versionStorage;

        public VersionHandler(InstallerOptions installerOptions, IVersionStorage versionStorage)
        {
            _installerOptions = installerOptions;
            _versionStorage = versionStorage;
        }

        internal virtual InstallationNameAndVersion GetInstalledVersion(string name)
        {
            if (!IsInstalled())
            {
                throw new InvalidOperationException("Installer is not installed");
            }
            
            InstallationNameAndVersion installedNameAndVersion = _versionStorage.Get(name);

            if (installedNameAndVersion == default(InstallationNameAndVersion))
            {
                installedNameAndVersion = new InstallationNameAndVersion
                {
                    InstallationName = name,
                    InstalledVersion = 0,
                    PreviousVersion = 0,
                    StartedInstallingVersion = 0
                };

                _versionStorage.Create(installedNameAndVersion);
            }

            return installedNameAndVersion;
        }

        internal virtual IEnumerable<InstallationNameAndVersion> GetInstalledVersionsInformation()
        {
            if (IsInstalled())
            {
                return _versionStorage.GetAll();
            }
            else
            {
                return Enumerable.Empty<InstallationNameAndVersion>();
            }
        }

        internal void InstallBaseVersion(InstallerBaseVersion installerBaseVersion)
        {
            var installedNameAndVersion = new InstallationNameAndVersion
            {
                InstallationName = installerBaseVersion.InstallationName,
                InstalledVersion = installerBaseVersion.Version,
                PreviousVersion = 0,
                StartedInstallingVersion = installerBaseVersion.Version
            };

            _versionStorage.Create(installedNameAndVersion);
        }

        internal bool IsInstalled()
        {
            return _versionStorage.IsInstalled();
        }

        internal void BeginInstallVersion(DatabaseVersion version)
        {
            // Get installation row from database
            var installedVersion = GetInstalledVersion(version.InstallationName);

            // Verify that this version really should be installed
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

            // Update information that this installation is beginning to install now, make sure that no one is in between, throw is someone is
            int result = _versionStorage.StartInstallation(installedVersion);

            if (result != 1)
            {
                throw new InvalidOperationException(string.Format("Starting installation for version {0} for {1} failed with count {2}", version.Version, version.InstallationName, result));
            }
        }

        internal void SetVersionInstalled(DatabaseVersion version)
        {
            // Get installation row from database
            var installedVersion = GetInstalledVersion(version.InstallationName);

            // Verify that this version really should have been installed now
            if (installedVersion.InstalledVersion >= version.Version)
            {
                throw new InvalidOperationException(string.Format("Version ({0}) is already installed for {1}", version.Version, version.InstallationName));
            }
            if (installedVersion.StartedInstallingVersion != version.Version)
            {
                throw new InvalidOperationException(string.Format("Version ({0}) is not in progress for {1} as it should be", version.Version, version.InstallationName));
            }

            // Update information that this installation is ended now, make sure that no one is in between, throw is someone is
            using (var connection = new SqlConnection(_installerOptions.ConnectionString))
            {
                int result = _versionStorage.EndInstallation(installedVersion);

                if (result != 1)
                {
                    throw new InvalidOperationException(string.Format("Starting installation for version {0} for {1} failed with count {2}", version.Version, version.InstallationName, result));
                }
            }
        }

        internal void UndoBeginInstallVersion(DatabaseVersion version)
        {
            // Get installation row from database
            var installedVersion = GetInstalledVersion(version.InstallationName);

            // Verify that this version installation beginning should be undone
            if (installedVersion.StartedInstallingVersion != version.Version)
            {
                throw new InvalidOperationException(string.Format("Version ({0}) has not started to install for {1}", version.Version, version.InstallationName));
            }

            if (installedVersion.InstalledVersion >= version.Version)
            {
                throw new InvalidOperationException(string.Format("This version ({0}) is already installed for {1}", version.Version, version.InstallationName));
            }

            // Update information that this installation is undoing the begin to install, make sure that no one is in between, throw is someone is
            int result = _versionStorage.UndoInstallation(installedVersion);

            if (result != 1)
            {
                throw new InvalidOperationException(string.Format("Undo begin install for version {0} for {1} failed with count {2}", version.Version, version.InstallationName, result));
            }
        }
    }
}