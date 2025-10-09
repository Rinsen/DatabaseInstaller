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
        private static Action<IServiceCollection>? _addServices;
        private static Type? _databaseSetupType;
        private static Type? _dataSeedType;

        internal static async Task StartDatabaseInstaller()
        {
            var databaseVersionsToInstall = new List<DatabaseVersion>();

            var serviceProvider = BootstrapApplication();

            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var logger = serviceProvider.GetRequiredService<ILogger<InstallationProgram>>();
            
            var completed = await InstallDatabase(databaseVersionsToInstall, serviceProvider, configuration, logger);

            if (completed && _dataSeedType is not null)
            {
                await SeedData(serviceProvider, configuration, logger);
            }

            logger.LogInformation($"Done");
        }

        private static async Task<bool> InstallDatabase(List<DatabaseVersion> databaseVersionsToInstall, ServiceProvider serviceProvider, IConfiguration configuration, ILogger<InstallationProgram> logger)
        {
            if (_databaseSetupType == null)
            {
                throw new InvalidOperationException("Database setup type not configured. Call AddDatabaseSetup() first.");
            }

            var installerstartup = (IDatabaseSetup)serviceProvider.GetRequiredService(_databaseSetupType);
            installerstartup.DatabaseVersionsToInstall(databaseVersionsToInstall, configuration);

            var installationHandler = serviceProvider.GetRequiredService<InstallationHandler>();
            try
            {
                if (!IsConfigurationValid(logger, configuration))
                {
                    return false;
                }

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
                        logger.LogInformation("Command is not supported {command}", configuration["Command"]);
                        break;
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to run installer");

                return false;
            }

            return true;
        }

        private static bool IsConfigurationValid(ILogger<InstallationProgram> logger, IConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration["Command"]))
            {
                logger.LogError("Command is required");
                return false;
            }

            if (string.IsNullOrEmpty(configuration["DatabaseName"]))
            {
                logger.LogError("DatabaseName is required");
                return false;
            }

            if (string.IsNullOrEmpty(configuration["Schema"]))
            {
                logger.LogError("Schema is required");
                return false;
            }

            var connectionStringName = configuration["ConnectionStringName"];
            if (string.IsNullOrEmpty(connectionStringName))
            {
                logger.LogError("ConnectionStringName is required");
                return false;
            }

            if (string.IsNullOrEmpty(configuration.GetConnectionString(connectionStringName)))
            {
                logger.LogError("ConnectionString is required");
                return false;
            }

            return true;
        }

        private static async Task SeedData(ServiceProvider serviceProvider, IConfiguration configuration, ILogger<InstallationProgram> logger)
        {
            if (_dataSeedType == null)
            {
                logger.LogWarning("Data seed type is not configured");
                return;
            }

            try
            {
                var dataSeed = (IDataSeed)serviceProvider.GetRequiredService(_dataSeedType);
                logger.LogInformation("Starting data seeding");
                await dataSeed.SeedData();
                logger.LogInformation("Data seeding completed successfully");
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to seed data");
                throw;
            }
        }

        private static ServiceProvider BootstrapApplication()
        {
            var environmentName = "Production";
#if DEBUG
            environmentName = "Development";
#endif
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                      .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
                      .AddEnvironmentVariables();

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
            
            var connectionStringName = config["ConnectionStringName"];
            if (string.IsNullOrEmpty(connectionStringName))
            {
                throw new InvalidOperationException("ConnectionStringName is required in configuration");
            }

            var connectionString = config.GetConnectionString(connectionStringName);
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException($"Connection string '{connectionStringName}' not found in configuration");
            }

            var databaseName = config["DatabaseName"];
            if (string.IsNullOrEmpty(databaseName))
            {
                throw new InvalidOperationException("DatabaseName is required in configuration");
            }

            var schema = config["Schema"];
            if (string.IsNullOrEmpty(schema))
            {
                throw new InvalidOperationException("Schema is required in configuration");
            }

            serviceCollection.AddSingleton(new InstallerOptions(databaseName, schema, connectionString, connectionStringName));

            if (_databaseSetupType != null)
            {
                serviceCollection.AddTransient(_databaseSetupType);
            }

            if (_dataSeedType != null)
            {
                serviceCollection.AddTransient(_dataSeedType);
            }
            
            serviceCollection.AddDatabaseInstaller();

            // Add custom services if configured
            _addServices?.Invoke(serviceCollection);

            return serviceCollection.BuildServiceProvider();
        }
        
        internal static void AddServices(Action<IServiceCollection> value)
        {
            _addServices = value;
        }

        internal static void AddDatabaseSetup<T>() where T : class, IDatabaseSetup, new()
        {
            _databaseSetupType = typeof(T);
        }

        internal static void AddDataSeed<T>() where T : class, IDataSeed
        {
            _dataSeedType = typeof(T);
        }
    }
}
