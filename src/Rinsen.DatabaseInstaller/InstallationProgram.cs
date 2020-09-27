using Microsoft.Extensions.Configuration;
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
            var databaseVersionsToInstall = new List<DatabaseVersion>();

            var serviceProvider = BootstrapApplication<T>();

            var installerstartup = serviceProvider.GetRequiredService<T>();
            var configuration = serviceProvider.GetService<IConfiguration>();

            installerstartup.DatabaseVersionsToInstall(databaseVersionsToInstall);

            var installationHandler = serviceProvider.GetRequiredService<InstallationHandler>();
            var logger = serviceProvider.GetRequiredService<Logger<InstallationProgram>>();

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

            logger.LogInformation($"Done");
        }

        private ServiceProvider BootstrapApplication<T>() where T : class
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddUserSecrets<T>();
            var config = configBuilder.Build();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging((builder) =>
            {
                builder.SetMinimumLevel(LogLevel.Information)
                        .AddFilter("Microsoft", LogLevel.Warning)
                        .AddFilter("System", LogLevel.Warning)
                        .AddConsole();
            })
            .AddSingleton(config);

            serviceCollection.AddTransient<InstallationHandler>();
            serviceCollection.AddSingleton<InstallerOptions>();
            serviceCollection.AddDatabaseInstaller();

            return serviceCollection.BuildServiceProvider();
        }
    }
}
