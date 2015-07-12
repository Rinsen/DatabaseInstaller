using Microsoft.Framework.DependencyInjection;
using System.Collections.Generic;
using Rinsen.DatabaseInstaller;
using Microsoft.Framework.Configuration;

namespace DatabaseInstallerSampleApp
{
    public class Program
    {
        public void Main(string[] args)
        {
            var config = new ConfigurationBuilder().AddJsonFile("config.json").Build();
            
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddDatabaseInstaller(config["Data:DefaultConnection:ConnectionString"]);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var installer = serviceProvider.GetRequiredService<Installer>();

            var versionList = new List<DatabaseVersion>();

            installer.Run(versionList);

        }
    }
}
