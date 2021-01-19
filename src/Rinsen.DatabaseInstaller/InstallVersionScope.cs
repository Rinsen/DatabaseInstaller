using System;
using System.Data.SqlClient;

namespace Rinsen.DatabaseInstaller
{
    internal class InstallVersionScope : IDisposable
    {
        private readonly DatabaseVersion _databaseVersion;
        private readonly SqlConnection _connection;
        private readonly SqlTransaction _transaction;
        private readonly IVersionStorage _versionStorage;
        private bool _failed = true;
        private Exception _e;

        public InstallVersionScope(IVersionStorage versionStorage, DatabaseVersion databaseVersion, SqlConnection connection, SqlTransaction transaction)
        {
            _databaseVersion = databaseVersion;
            _connection = connection;
            _transaction = transaction;
            _versionStorage = versionStorage;
        }

        public void Dispose()
        {
            if (_failed)
            {
                throw new InvalidOperationException($"Starting installation for version { _databaseVersion.Version} for {_databaseVersion.InstallationName}", _e);
            }

            var installedVersion = GetCurrentInstalledVersionAndValidatePostInstallationState();

            if (installedVersion == null)
                return;

            // Update information that this installation is ended now, make sure that no one is in between, throw is someone is
            int result = _versionStorage.EndInstallation(installedVersion, _connection, _transaction);

            if (result != 1)
            {
                throw new InvalidOperationException(string.Format("Starting installation for version {0} for {1} failed with count {2}", _databaseVersion.Version, _databaseVersion.InstallationName, result));
            }
        }

        private InstallationNameAndVersion GetCurrentInstalledVersionAndValidatePostInstallationState()
        {
            // Get installation row from database
            var installedVersion = _versionStorage.Get(_databaseVersion.InstallationName, _connection, _transaction);

            if (installedVersion == null)
                return null;

            // Verify that this version really should have been installed now
            if (installedVersion.InstalledVersion >= _databaseVersion.Version)
            {
                throw new InvalidOperationException(string.Format("Version ({0}) is already installed for {1}", _databaseVersion.Version, _databaseVersion.InstallationName));
            }
            if (installedVersion.StartedInstallingVersion != _databaseVersion.Version)
            {
                throw new InvalidOperationException(string.Format("Version ({0}) is not in progress for {1} as it should be", _databaseVersion.Version, _databaseVersion.InstallationName));
            }

            return installedVersion;
        }

        internal void Complete()
        {
            _failed = false;
        }

        internal void Fail(Exception e)
        {
            _e = e;
        }
    }
}
