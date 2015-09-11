//using Dapper;
//using System;
//using System.Collections.Generic;
//using System.Data.SqlClient;
//using System.Linq;

//namespace Rinsen.DatabaseInstaller
//{
//    public class DapperVersionStorage : IVersionStorage, IDisposable
//    {
//        readonly InstallerOptions _installerOptions;
//        SqlConnection _connection;

//        public DapperVersionStorage(InstallerOptions installerOptions)
//        {
//            _installerOptions = installerOptions;
//            _connection = new SqlConnection(installerOptions.ConnectionString);
//        }

//        public void Create(InstallationNameAndVersion installedNameAndVersion)
//        {
//            string insertSql = string.Format(@"INSERT INTO {0} (InstallationName, PreviousVersion, StartedInstallingVersion, InstalledVersion) VALUES (@InstallationName, @PreviousVersion, @StartedInstallingVersion, @InstalledVersion); SELECT CAST(SCOPE_IDENTITY() as int)", _installerOptions.InstalledVersionsDatabaseTableName);

//            installedNameAndVersion.Id = _connection.Query<int>(insertSql, installedNameAndVersion).Single();
//        }

//        public InstallationNameAndVersion Get(string name)
//        {
//            return _connection.Query<InstallationNameAndVersion>(string.Format("SELECT * FROM {0} WHERE InstallationName = @InstallationName ", _installerOptions.InstalledVersionsDatabaseTableName), new { installationName = name }).FirstOrDefault();
//        }

//        public IEnumerable<InstallationNameAndVersion> GetAll()
//        {
//            return _connection.Query<InstallationNameAndVersion>(string.Format("SELECT * FROM {0}", _installerOptions.InstalledVersionsDatabaseTableName));
//        }

//        public int StartInstallation(InstallationNameAndVersion installedVersion)
//        {
//            var updateSql = string.Format("UPDATE {0} SET StartedInstallingVersion = @StartedInstallingVersion + 1 WHERE Id = @id AND PreviousVersion = @PreviousVersion AND StartedInstallingVersion = @StartedInstallingVersion AND InstalledVersion = @InstalledVersion", _installerOptions.InstalledVersionsDatabaseTableName);
//            return _connection.Execute(updateSql, installedVersion);
//        }

//        public int EndInstallation(InstallationNameAndVersion installedVersion)
//        {
//            var updateSql = string.Format("UPDATE {0} SET InstalledVersion = @InstalledVersion + 1 WHERE Id = @id AND PreviousVersion = @PreviousVersion AND StartedInstallingVersion = @StartedInstallingVersion AND InstalledVersion = @InstalledVersion", _installerOptions.InstalledVersionsDatabaseTableName);
//            return _connection.Execute(updateSql, installedVersion);
//        }

//        public int UndoInstallation(InstallationNameAndVersion installedVersion)
//        {
//            var updateSql = string.Format("UPDATE {0} SET StartedInstallingVersion = @StartedInstallingVersion - 1 WHERE Id = @id AND PreviousVersion = @PreviousVersion AND StartedInstallingVersion = @StartedInstallingVersion AND InstalledVersion = @InstalledVersion", _installerOptions.InstalledVersionsDatabaseTableName);
//            return _connection.Execute(updateSql, installedVersion);
//        }

//        public bool IsInstalled()
//        {
//            var data = _connection.Query("SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @tableName", new { tableName = _installerOptions.InstalledVersionsDatabaseTableName });

//            if (data.Count() >= 5)
//            {
//                return true;
//            }
//            return false;
//        }

//        public void Dispose()
//        {
//            _connection.Dispose();
//        }
//    }
//}
