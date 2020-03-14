using Rinsen.DatabaseInstaller;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DatabaseInstallerSampleApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                await RunDatabaseInstaller();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }

        private static async Task RunDatabaseInstaller()
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddUserSecrets<Program>();

            var config = configBuilder.Build();

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddDatabaseInstaller(config["Data:DefaultConnection:ConnectionString"]);
            serviceCollection.AddLogging(builder => {
                builder.AddFilter("Rinsen", LogLevel.Debug);
                builder.AddConsole();
            });

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var installer = serviceProvider.GetRequiredService<Installer>();

            var info = installer.GetVersionInformation();

            // Install installer and first version
            var versionList = new List<DatabaseVersion>
            {
                new FirstTableVersion()
            };

            await installer.RunAsync(versionList);

            // Install failing script and se that the rollback is working
            var secondTable = new SecondTableVersion();
            var exceptionFound = false;
            try
            {
                versionList.Add(secondTable);
                await installer.RunAsync(versionList);
            }
            catch (CommandFailedToExecuteException)
            {
                exceptionFound = true;
                var installedVersion = (await installer.GetVersionInformation()).Single(m => m.InstallationName == secondTable.InstallationName);

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
            await installer.RunAsync(versionList);

            // Run with installation list containing previous version
            versionList.Add(new ThirdVersion());
            await installer.RunAsync(versionList);
        }
    }
}