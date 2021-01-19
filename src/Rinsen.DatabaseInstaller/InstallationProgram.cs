﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rinsen.DatabaseInstaller.ConsoleInstaller;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rinsen.DatabaseInstaller
{
    internal class InstallationProgram
    {

        public async Task StartDatabaseInstaller<T>() where T : class, IInstallerStartup, new()
        {
            var databaseVersionsToInstall = new List<DatabaseSettingsVersion>();

            var serviceProvider = BootstrapApplication<T>();

            var installerstartup = serviceProvider.GetRequiredService<T>();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            installerstartup.DatabaseVersionsToInstall(databaseVersionsToInstall);

            var installationHandler = serviceProvider.GetService<InstallationHandler>();
            var logger = serviceProvider.GetService<ILogger<InstallationProgram>>();
            
            try
            {
                switch (configuration["Command"])
                {
                    case "Install":
                        await installationHandler.Install(databaseVersionsToInstall);
                        break;
                    case "Preview":
                        await installationHandler.PreviewDbChanges(databaseVersionsToInstall);
                        break;
                    case "ShowAll":
                        installationHandler.AllDbChanges(databaseVersionsToInstall);
                        break;
                    case "CurrentState":
                        await installationHandler.ShowCurrentInstallationState();
                        break;
                    default:
                        logger.LogInformation("Valid command is required");
                        break;
                }


            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to run installer");
            }

            logger.LogInformation($"Done");
            Console.ReadKey();
        }

        private ServiceProvider BootstrapApplication<T>() where T : class
        {
            var environmentName = "Production";
#if DEBUG
            environmentName = "Development";
#endif
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddUserSecrets<T>();
            configBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                      .AddJsonFile($"appsettings.{environmentName}.json",
                                     optional: false, reloadOnChange: true);

            var config = configBuilder.Build();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging((builder) =>
            {
                builder.SetMinimumLevel(LogLevel.Information)
                        .AddFilter("Microsoft", LogLevel.Warning)
                        .AddFilter("System", LogLevel.Warning)
                        .AddConsole();
            });

            serviceCollection.AddSingleton<IConfiguration>(config);
            var databaseName = config["DatabaseName"];
            serviceCollection.AddSingleton(new InstallerOptions
            {
                ConnectionString = config.GetConnectionString(databaseName),
                DatabaseName = databaseName,
                Schema = config["Schema"]
            });

            serviceCollection.AddTransient<T>();
            serviceCollection.AddDatabaseInstaller();

            return serviceCollection.BuildServiceProvider();
        }
    }
}
