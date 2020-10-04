using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Transactions;

namespace Rinsen.DatabaseInstaller
{
    internal class AdoNetVersionStorage : IVersionStorage
    {
        private readonly InstallerOptions _installerOptions;

        public AdoNetVersionStorage(InstallerOptions installerOptions)
        {
            _installerOptions = installerOptions;
        }

        public async Task Create(InstallationNameAndVersion installedNameAndVersion, SqlConnection connection, SqlTransaction transaction)
        {
            string insertSql = $@"INSERT INTO [{_installerOptions.DatabaseName}].[{_installerOptions.Schema}].[{InstallerConstants.InstalledVersionsDatabaseTableName}] (InstallationName, PreviousVersion, StartedInstallingVersion, InstalledVersion) VALUES (@InstallationName, @PreviousVersion, @StartedInstallingVersion, @InstalledVersion); SELECT CAST(SCOPE_IDENTITY() as int)";
            using (var command = new SqlCommand(insertSql, connection, transaction))
            {
                command.Parameters.Add(new SqlParameter("@InstallationName", installedNameAndVersion.InstallationName));
                command.Parameters.Add(new SqlParameter("@InstalledVersion", installedNameAndVersion.InstalledVersion));
                command.Parameters.Add(new SqlParameter("@PreviousVersion", installedNameAndVersion.PreviousVersion));
                command.Parameters.Add(new SqlParameter("@StartedInstallingVersion", installedNameAndVersion.StartedInstallingVersion));

                installedNameAndVersion.Id = (int)await command.ExecuteScalarAsync();
            }
        }

        public InstallationNameAndVersion Get(string name, SqlConnection connection, SqlTransaction transaction)
        {
            var result = default(InstallationNameAndVersion);

            using (var command = new SqlCommand($"SELECT * FROM [{_installerOptions.DatabaseName}].[{_installerOptions.Schema}].[{InstallerConstants.InstalledVersionsDatabaseTableName}] WHERE InstallationName = @InstallationName", connection, transaction))
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

        public async Task<InstallationNameAndVersion> GetAsync(string name, SqlConnection connection, SqlTransaction transaction)
        {
            var result = default(InstallationNameAndVersion);

            using (var command = new SqlCommand($"SELECT * FROM [{_installerOptions.DatabaseName}].[{_installerOptions.Schema}].[{InstallerConstants.InstalledVersionsDatabaseTableName}] WHERE InstallationName = @InstallationName", connection, transaction))
            {
                command.Parameters.Add(new SqlParameter("@InstallationName", name));
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
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

        public async Task<IEnumerable<InstallationNameAndVersion>> GetAll(SqlConnection connection, SqlTransaction transaction)
        {
            var results = new List<InstallationNameAndVersion>();

            using (var command = new SqlCommand($"SELECT * FROM [{_installerOptions.DatabaseName}].[{_installerOptions.Schema}].[{InstallerConstants.InstalledVersionsDatabaseTableName}]", connection, transaction))
            {
                using (var reader = await command.ExecuteReaderAsync())
                { 
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
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

        public async Task<bool> IsInstalled(SqlConnection connection)
        {
            using (var command = new SqlCommand($"SELECT * FROM [{_installerOptions.DatabaseName}].INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = @schema AND TABLE_NAME = @tableName", connection))
            {
                command.Parameters.Add(new SqlParameter("@tableName", InstallerConstants.InstalledVersionsDatabaseTableName));
                command.Parameters.Add(new SqlParameter("@schema", _installerOptions.Schema));

                using (var reader = await command.ExecuteReaderAsync())
                {
                    return reader.HasRows;
                }
            }
        }

        public async Task<bool> IsInstalled(SqlConnection connection, SqlTransaction sqlTransaction)
        {
            using (var command = new SqlCommand($"SELECT * FROM [{_installerOptions.DatabaseName}].INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = @schema AND TABLE_NAME = @tableName", connection, sqlTransaction))
            {
                command.Parameters.Add(new SqlParameter("@tableName", InstallerConstants.InstalledVersionsDatabaseTableName));
                command.Parameters.Add(new SqlParameter("@schema", _installerOptions.Schema));

                using (var reader = await command.ExecuteReaderAsync())
                {
                    return reader.HasRows;
                }
            }
        }

        public Task<int> StartInstallation(InstallationNameAndVersion installedVersion, SqlConnection connection, SqlTransaction transaction)
        {
            var updateSql = $"UPDATE [{_installerOptions.DatabaseName}].[{_installerOptions.Schema}].[{InstallerConstants.InstalledVersionsDatabaseTableName}] SET StartedInstallingVersion = @StartedInstallingVersion + 1 WHERE Id = @Id AND PreviousVersion = @PreviousVersion AND StartedInstallingVersion = @StartedInstallingVersion AND InstalledVersion = @InstalledVersion";
            using (var command = new SqlCommand(updateSql, connection, transaction))
            {
                command.Parameters.Add(new SqlParameter("@Id", installedVersion.Id));
                command.Parameters.Add(new SqlParameter("@PreviousVersion", installedVersion.PreviousVersion));
                command.Parameters.Add(new SqlParameter("@StartedInstallingVersion", installedVersion.StartedInstallingVersion));
                command.Parameters.Add(new SqlParameter("@InstalledVersion", installedVersion.InstalledVersion));

                return command.ExecuteNonQueryAsync();
            }
        }

        public int EndInstallation(InstallationNameAndVersion installedVersion, SqlConnection connection, SqlTransaction transaction)
        {
            var updateSql = $"UPDATE [{_installerOptions.DatabaseName}].[{_installerOptions.Schema}].[{InstallerConstants.InstalledVersionsDatabaseTableName}] SET InstalledVersion = @InstalledVersion + 1 WHERE Id = @id AND PreviousVersion = @PreviousVersion AND StartedInstallingVersion = @StartedInstallingVersion AND InstalledVersion = @InstalledVersion";
            using (var command = new SqlCommand(updateSql, connection, transaction))
            {
                command.Parameters.Add(new SqlParameter("@Id", installedVersion.Id));
                command.Parameters.Add(new SqlParameter("@PreviousVersion", installedVersion.PreviousVersion));
                command.Parameters.Add(new SqlParameter("@StartedInstallingVersion", installedVersion.StartedInstallingVersion));
                command.Parameters.Add(new SqlParameter("@InstalledVersion", installedVersion.InstalledVersion));

                return command.ExecuteNonQuery();
            }
        }

        public Task<int> EndInstallationAsync(InstallationNameAndVersion installedVersion, SqlConnection connection, SqlTransaction transaction)
        {
            var updateSql = $"UPDATE [{_installerOptions.DatabaseName}].[{_installerOptions.Schema}].[{InstallerConstants.InstalledVersionsDatabaseTableName}] SET InstalledVersion = @InstalledVersion + 1 WHERE Id = @id AND PreviousVersion = @PreviousVersion AND StartedInstallingVersion = @StartedInstallingVersion AND InstalledVersion = @InstalledVersion";
            using (var command = new SqlCommand(updateSql, connection, transaction))
            {
                command.Parameters.Add(new SqlParameter("@Id", installedVersion.Id));
                command.Parameters.Add(new SqlParameter("@PreviousVersion", installedVersion.PreviousVersion));
                command.Parameters.Add(new SqlParameter("@StartedInstallingVersion", installedVersion.StartedInstallingVersion));
                command.Parameters.Add(new SqlParameter("@InstalledVersion", installedVersion.InstalledVersion));

                return command.ExecuteNonQueryAsync();
            }
        }
    }
}
