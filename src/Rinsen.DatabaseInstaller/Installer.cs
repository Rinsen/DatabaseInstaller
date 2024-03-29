﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Rinsen.DatabaseInstaller
{
    internal class Installer
    {
        private readonly DatabaseVersionInstaller _databaseVersionInstaller;
        private readonly DatabaseInitializer _databaseInitializer;
        private readonly VersionHandler _versionHandler;
        private readonly ILogger<Installer> _log;
        private readonly InstallerOptions _installerOptions;

        public Installer(DatabaseVersionInstaller databaseVersionInstaller,
            DatabaseInitializer databaseInitializer,
            VersionHandler versionHandler,
            InstallerOptions installerOptions,
            ILogger<Installer> log)
        {
            _databaseVersionInstaller = databaseVersionInstaller;
            _databaseInitializer = databaseInitializer;
            _versionHandler = versionHandler;
            _installerOptions = installerOptions;
            _log = log;
        }
        
        public async Task RunAsync(List<DatabaseVersion> databaseVersions)
        {
            await WaitForDatabaseConnection();

            _log.LogInformation("DatabaseInstaller started");

            using var connection = new SqlConnection(_installerOptions.ConnectionString);

            await connection.OpenAsync();

            await _databaseInitializer.InitializeAsync(connection);

            using var transaction = connection.BeginTransaction();

            await _databaseVersionInstaller.InstallAsync(databaseVersions, connection, transaction);
            
            _log.LogDebug("DatabaseInstaller finished, commit changes");

            transaction.Commit();
            _log.LogDebug("Commit completed");
        }

        private async Task WaitForDatabaseConnection()
        {
            var fail = 1;
            while (true)
            {
                try
                {
                    using var connection = new SqlConnection(_installerOptions.ConnectionString);
                    
                    await connection.OpenAsync();

                    return;
                }
                catch (Exception e)
                {
                    _log.LogInformation(e, "Failed to connect to database {count} times", fail);
                    if (e.InnerException is not null)
                    {
                        _log.LogInformation(e.InnerException, "Failed to connect to database inner");

                        if (e.InnerException.InnerException is not null)
                        {
                            _log.LogInformation(e.InnerException.InnerException, "Failed to connect to database inner inner");
                        }
                    }
                    
                    fail++;
                    if (fail == 1000)
                    {
                        throw new Exception("Failed to connect to SQL Server", e);
                    }

                    await Task.Delay(TimeSpan.FromSeconds(10));
                }
            }
        }

        public void RollbackVersions(string name, int toVersion)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<InstallationNameAndVersion>> GetVersionInformationAsync()
        {
            using var connection = new SqlConnection(_installerOptions.ConnectionString);
            
            await connection.OpenAsync();
            
            using var transaction = connection.BeginTransaction();
            
            return await _versionHandler.GetInstalledVersionsInformation(connection, transaction);
        } 
    }
}
