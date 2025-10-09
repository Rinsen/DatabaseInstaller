using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rinsen.DatabaseInstaller;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InstallationSampleConsoleApp
{
    class Program
    {
        static Task Main(string[] args)
        {
            var installerHostBuilder = InstallerHost.CreateBuilder();
            
            installerHostBuilder.AddServices(services =>
            {
               // Add application specific services here
               services.AddSingleton<Dependency>();
            });

            installerHostBuilder.AddDatabaseSetup<DatabaseSetup>();

            installerHostBuilder.AddDataSeed<DataSeed>();

            return installerHostBuilder.Start();
        }
    }

    public class DatabaseSetup : IDatabaseSetup
    {
        public void DatabaseVersionsToInstall(List<DatabaseVersion> databaseVersions, IConfiguration configuration)
        {
            databaseVersions.Add(new SetDatabaseSettingsVersion(configuration));
            databaseVersions.Add(new CreateTables());
        }
    }

    public class DataSeed : IDataSeed
    {
        private readonly InstallerOptions _installerOptions;
        private readonly Dependency _dependency;

        public DataSeed(InstallerOptions installerOptions,
        Dependency dependency)
        {
            _installerOptions = installerOptions;
            _dependency = dependency;
        }

        public async Task SeedData()
        {
            Console.WriteLine("Seeding data...");

            using var connection = new SqlConnection(_installerOptions.ConnectionString);
            await connection.OpenAsync();

            // Check if data already exists to avoid duplicate seeding
            var checkQuery = $"SELECT COUNT(*) FROM [{_installerOptions.DatabaseName}].[{_installerOptions.Schema}].[NullableDatas]";
            using var checkCommand = new SqlCommand(checkQuery, connection);
            var existingRowCount = (int)await checkCommand.ExecuteScalarAsync();

            if (existingRowCount >= _dependency.CreateCount)
            {
                Console.WriteLine($"Data already exists ({existingRowCount} rows). Skipping seeding.");
                return;
            }

            // Seed 10 rows of data
            var insertQuery = $@"
                INSERT INTO [{_installerOptions.DatabaseName}].[{_installerOptions.Schema}].[NullableDatas] 
                (NotNullableBool, NullableBool, NullableByte, NullableByteArray, NullableDateTime, 
                 NullableDateTimeOffset, NullableDecimal, NullableDouble, NullableGuid, NullableInt, 
                 NullableLong, NullableShort) 
                VALUES 
                (@NotNullableBool, @NullableBool, @NullableByte, @NullableByteArray, @NullableDateTime, 
                 @NullableDateTimeOffset, @NullableDecimal, @NullableDouble, @NullableGuid, @NullableInt, 
                 @NullableLong, @NullableShort)";

            var count = _dependency.CreateCount - existingRowCount;
            for (int i = 1; i <= count; i++)
            {
                using var insertCommand = new SqlCommand(insertQuery, connection);

                // Add parameters with varied data including some nulls
                insertCommand.Parameters.AddWithValue("@NotNullableBool", i % 2 == 0);
                insertCommand.Parameters.AddWithValue("@NullableBool", i % 3 == 0 ? DBNull.Value : (object)(i % 2 == 0));
                insertCommand.Parameters.AddWithValue("@NullableByte", i % 4 == 0 ? DBNull.Value : (object)(byte)(i * 10));

                // Handle byte array properly for varbinary column
                if (i % 5 == 0)
                {
                    insertCommand.Parameters.Add("@NullableByteArray", System.Data.SqlDbType.VarBinary).Value = DBNull.Value;
                }
                else
                {
                    insertCommand.Parameters.Add("@NullableByteArray", System.Data.SqlDbType.VarBinary).Value = new byte[] { (byte)i, (byte)(i * 2) };
                }

                insertCommand.Parameters.AddWithValue("@NullableDateTime", i % 6 == 0 ? DBNull.Value : (object)DateTime.Now.AddDays(i));
                insertCommand.Parameters.AddWithValue("@NullableDateTimeOffset", i % 7 == 0 ? DBNull.Value : (object)DateTimeOffset.Now.AddHours(i));
                insertCommand.Parameters.AddWithValue("@NullableDecimal", i % 8 == 0 ? DBNull.Value : (object)(decimal)(i * 100.50m));
                insertCommand.Parameters.AddWithValue("@NullableDouble", i % 9 == 0 ? DBNull.Value : (object)(double)(i * 3.14));
                insertCommand.Parameters.AddWithValue("@NullableGuid", i % 10 == 0 ? DBNull.Value : (object)Guid.NewGuid());
                insertCommand.Parameters.AddWithValue("@NullableInt", i % 2 == 0 ? DBNull.Value : (object)(i * 1000));
                insertCommand.Parameters.AddWithValue("@NullableLong", i % 3 == 0 ? DBNull.Value : (object)((long)i * 1000000));
                insertCommand.Parameters.AddWithValue("@NullableShort", i % 4 == 0 ? DBNull.Value : (object)(short)(i * 10));

                await insertCommand.ExecuteNonQueryAsync();
            }

            Console.WriteLine($"Successfully seeded {count} rows of data into NullableDatas table.");
        }
    }

    public class Dependency
    {
        public int CreateCount { get; set; } = 20;
    }
}
