using System;
using System.Data.SqlClient;
using Dapper;
using System.Linq;
using System.Collections.Generic;

namespace Rinsen.DatabaseInstaller
{
    public class VersionHandler
    {
        private readonly InstallerOptions _installerOptions;

        public VersionHandler(InstallerOptions installerOptions)
        {
            _installerOptions = installerOptions;
        }

        internal virtual InstallationNameAndVersion GetInstalledVersion(string name)
        {
            if (!IsInstalled())
            {
                throw new InvalidOperationException("Installer is not installed");
            }

            using (var connection = new SqlConnection(_installerOptions.ConnectionString))
            {
                var installedNameAndVersion = connection.Query<InstallationNameAndVersion>(string.Format("SELECT * FROM {0} WHERE InstallationName = @InstallationName ", _installerOptions.InstalledVersionsDatabaseTableName), new { installationName = name }).FirstOrDefault();

                if (installedNameAndVersion == default(InstallationNameAndVersion))
                {
                    installedNameAndVersion = new InstallationNameAndVersion
                    {
                        InstallationName = name,
                        InstalledVersion = 0,
                        PreviousVersion = 0,
                        StartedInstallingVersion = 0
                    };

                    string insertSql = string.Format(@"INSERT INTO {0} (InstallationName, PreviousVersion, StartedInstallingVersion, InstalledVersion) VALUES (@InstallationName, @PreviousVersion, @StartedInstallingVersion, @InstalledVersion); SELECT CAST(SCOPE_IDENTITY() as int)", _installerOptions.InstalledVersionsDatabaseTableName);

                    installedNameAndVersion.Id = connection.Query<int>(insertSql, installedNameAndVersion).Single();
                }

                return installedNameAndVersion;
            }
        }

        internal virtual IEnumerable<InstallationNameAndVersion> GetInstalledVersionsInformation()
        {
            if (IsInstalled())
            {
                using (var connection = new SqlConnection(_installerOptions.ConnectionString))
                {
                    return connection.Query<InstallationNameAndVersion>(string.Format("SELECT * FROM {0}", _installerOptions.InstalledVersionsDatabaseTableName));
                }
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
            string insertSql = string.Format(@"INSERT INTO {0} (InstallationName, PreviousVersion, StartedInstallingVersion, InstalledVersion) VALUES (@InstallationName, @PreviousVersion, @StartedInstallingVersion, @InstalledVersion); SELECT CAST(SCOPE_IDENTITY() as int)", _installerOptions.InstalledVersionsDatabaseTableName);
            using (var connection = new SqlConnection(_installerOptions.ConnectionString))
            {
                installedNameAndVersion.Id = connection.Query<int>(insertSql, installedNameAndVersion).Single();
            }
        }

        internal bool IsInstalled()
        {
            using (var connection = new SqlConnection(_installerOptions.ConnectionString))
            {
                var data = connection.Query("SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @tableName", new { tableName = _installerOptions.InstalledVersionsDatabaseTableName });

                if (data.Count() >= 5)
                {
                    return true;
                }
                return false;
            }
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
            using (var connection = new SqlConnection(_installerOptions.ConnectionString))
            {
                var updateSql = string.Format("UPDATE {0} SET StartedInstallingVersion = @StartedInstallingVersion + 1 WHERE Id = @id AND PreviousVersion = @PreviousVersion AND StartedInstallingVersion = @StartedInstallingVersion AND InstalledVersion = @InstalledVersion", _installerOptions.InstalledVersionsDatabaseTableName);
                var result = connection.Execute(updateSql, installedVersion);

                if (result != 1)
                {
                    throw new InvalidOperationException(string.Format("Starting installation for version {0} for {1} failed with count {2}", version.Version, version.InstallationName, result));
                }
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
                var updateSql = string.Format("UPDATE {0} SET InstalledVersion = @InstalledVersion + 1 WHERE Id = @id AND PreviousVersion = @PreviousVersion AND StartedInstallingVersion = @StartedInstallingVersion AND InstalledVersion = @InstalledVersion", _installerOptions.InstalledVersionsDatabaseTableName);
                var result = connection.Execute(updateSql, installedVersion);

                if (result != 1)
                {
                    throw new InvalidOperationException(string.Format("Starting installation for version {0} for {1} failed with count {2}", version.Version, version.InstallationName, result));
                }
            }
        }
    }
}