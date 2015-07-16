using Microsoft.Framework.DependencyInjection;
using System.Collections.Generic;
using Rinsen.DatabaseInstaller;
using Microsoft.Framework.Configuration;
using System;
using System.Linq;

namespace DatabaseInstallerSampleApp
{
    public class Program
    {
        public void Main(string[] args)
        {
            var config = new ConfigurationBuilder("C:/Config").AddJsonFile("config.json").Build();
            
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddDatabaseInstaller(config["Data:DefaultConnection:ConnectionString"]);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var installer = serviceProvider.GetRequiredService<Installer>();

            var versionList = new List<DatabaseVersion>();
            versionList.Add(new FirstTableVersion());

            installer.Run(versionList);

            var secondTable = new SecondTableVersion();
            var exceptionFound = false;
            try
            {
                versionList.Add(secondTable);
                installer.Run(versionList);
            }
            catch (SqlCommandFailedToExecuteException)
            {
                exceptionFound = true;
                var installedVersion = installer.GetVersionInformation().Single(m => m.InstallationName == secondTable.InstallationName);

                if (installedVersion.InstalledVersion != 1 || installedVersion.InstalledVersion != installedVersion.StartedInstallingVersion)
                {
                    throw new Exception("This should not happen");
                }
            }

            if (!exceptionFound)
            {
                throw new Exception("This should not happen number two");
            }

            
        }
    }
}
