using System.Collections.Generic;
using Rinsen.DatabaseInstaller;
using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DatabaseInstallerSampleApp
{
    public class Program
    {
        public void Main(string[] args)
        {
            var config = new ConfigurationBuilder().SetBasePath("C:/Config").AddJsonFile("config.json").Build();
            
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddDatabaseInstaller(config["Data:DefaultConnection:ConnectionString"]);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var installer = serviceProvider.GetRequiredService<Installer>();

            var info = installer.GetVersionInformation();

            // Install installer and first version
            var versionList = new List<DatabaseVersion>();
            versionList.Add(new FirstTableVersion());

            installer.Run(versionList);

            // Install failing script and se that the rollback is working
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

            // Install alteration only, clear out failing script
            versionList.Clear();

            versionList.Add(new CorrectSecondVersion());
            installer.Run(versionList);

            // Run with installation list containing previous version
            versionList.Add(new ThirdVersion());
            installer.Run(versionList);
        }
    }
}
