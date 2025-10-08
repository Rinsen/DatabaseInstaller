using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rinsen.DatabaseInstaller;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        private readonly Dependency _dependency;

        public DataSeed(Dependency dependency)
        {
            _dependency = dependency;
        }

        public Task SeedData()
        {
            throw new System.NotImplementedException();
        }
    }

    public class Dependency
    {

    }
}
