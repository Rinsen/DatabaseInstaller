using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Rinsen.DatabaseInstaller
{
    public class AdoNetVersionStorage : IVersionStorage
    {
        readonly InstallerOptions _installerOptions;

        public AdoNetVersionStorage(InstallerOptions installerOptions)
        {
            _installerOptions = installerOptions;
        }

        public void Create(InstallationNameAndVersion installedNameAndVersion, SqlConnection connection, SqlTransaction transaction)
        {
            string insertSql = string.Format(@"INSERT INTO {0} (InstallationName, PreviousVersion, StartedInstallingVersion, InstalledVersion) VALUES (@InstallationName, @PreviousVersion, @StartedInstallingVersion, @InstalledVersion); SELECT CAST(SCOPE_IDENTITY() as int)", _installerOptions.InstalledVersionsDatabaseTableName);
            using (var command = new SqlCommand(insertSql, connection, transaction))
            {
                command.Parameters.Add(new SqlParameter("@InstallationName", installedNameAndVersion.InstallationName));
                command.Parameters.Add(new SqlParameter("@InstalledVersion", installedNameAndVersion.InstalledVersion));
                command.Parameters.Add(new SqlParameter("@PreviousVersion", installedNameAndVersion.PreviousVersion));
                command.Parameters.Add(new SqlParameter("@StartedInstallingVersion", installedNameAndVersion.StartedInstallingVersion));

                installedNameAndVersion.Id = (int)command.ExecuteScalar();
            }
        }

        public InstallationNameAndVersion Get(string name, SqlConnection connection, SqlTransaction transaction)
        {
            var result = default(InstallationNameAndVersion);

            using (var command = new SqlCommand(string.Format("SELECT * FROM {0} WHERE InstallationName = @InstallationName", _installerOptions.InstalledVersionsDatabaseTableName), connection, transaction))
            {
                command.Parameters.Add(new SqlParameter("@InstallationName", name));
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result = new InstallationNameAndVersion
                            {
                                Id = (int)reader["Id"],
                                InstallationName = (string)reader["InstallationName"],
                                InstalledVersion = (int)reader["InstalledVersion"],
                                PreviousVersion = (int)reader["PreviousVersion"],
                                StartedInstallingVersion = (int)reader["StartedInstallingVersion"]
                            };
                        }
                    }
                }
            }

            return result;
        }

        public IEnumerable<InstallationNameAndVersion> GetAll(SqlConnection connection, SqlTransaction transaction)
        {
            var results = new List<InstallationNameAndVersion>();

            using (var command = new SqlCommand(string.Format("SELECT * FROM {0}", _installerOptions.InstalledVersionsDatabaseTableName), connection, transaction))
            {
                using (var reader = command.ExecuteReader())
                { 
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            results.Add(new InstallationNameAndVersion
                            {
                                Id = (int)reader["Id"],
                                InstallationName = (string)reader["InstallationName"],
                                InstalledVersion = (int)reader["InstalledVersion"],
                                PreviousVersion = (int)reader["PreviousVersion"],
                                StartedInstallingVersion = (int)reader["StartedInstallingVersion"]
                            });
                        }
                    }
                }
            }

            return results;
        }

        public bool IsInstalled(SqlConnection connection, SqlTransaction transaction)
        {
            bool result = false;

            using (var command = new SqlCommand("SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @tableName", connection, transaction))
            {
                command.Parameters.Add(new SqlParameter("@tableName", _installerOptions.InstalledVersionsDatabaseTableName));

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        int count = 0;
                        while (reader.Read())
                       {
                            count++;
                        }

                        if (count >= 5)
                        {
                            result = true;
                        }
                    }
                }
            }

            return result;
        }

        public int StartInstallation(InstallationNameAndVersion installedVersion, SqlConnection connection, SqlTransaction transaction)
        {
            var updateSql = string.Format("UPDATE {0} SET StartedInstallingVersion = @StartedInstallingVersion + 1 WHERE Id = @Id AND PreviousVersion = @PreviousVersion AND StartedInstallingVersion = @StartedInstallingVersion AND InstalledVersion = @InstalledVersion", _installerOptions.InstalledVersionsDatabaseTableName);
            using (var command = new SqlCommand(updateSql, connection, transaction))
            {
                command.Parameters.Add(new SqlParameter("@Id", installedVersion.Id));
                command.Parameters.Add(new SqlParameter("@PreviousVersion", installedVersion.PreviousVersion));
                command.Parameters.Add(new SqlParameter("@StartedInstallingVersion", installedVersion.StartedInstallingVersion));
                command.Parameters.Add(new SqlParameter("@InstalledVersion", installedVersion.InstalledVersion));

                return command.ExecuteNonQuery();
            }
        }

        public int EndInstallation(InstallationNameAndVersion installedVersion, SqlConnection connection, SqlTransaction transaction)
        {
            var updateSql = string.Format("UPDATE {0} SET InstalledVersion = @InstalledVersion + 1 WHERE Id = @id AND PreviousVersion = @PreviousVersion AND StartedInstallingVersion = @StartedInstallingVersion AND InstalledVersion = @InstalledVersion", _installerOptions.InstalledVersionsDatabaseTableName);
            using (var command = new SqlCommand(updateSql, connection, transaction))
            {
                command.Parameters.Add(new SqlParameter("@Id", installedVersion.Id));
                command.Parameters.Add(new SqlParameter("@PreviousVersion", installedVersion.PreviousVersion));
                command.Parameters.Add(new SqlParameter("@StartedInstallingVersion", installedVersion.StartedInstallingVersion));
                command.Parameters.Add(new SqlParameter("@InstalledVersion", installedVersion.InstalledVersion));

                return command.ExecuteNonQuery();
            }
        }
    }
}
