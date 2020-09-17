using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rinsen.DatabaseInstaller;
using Rinsen.DatabaseInstaller.ConsoleInstaller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace InstallationSampleConsoleApp
{
    class Program : InstallationProgram
    {
        static Task Main(string[] args)
        {
            var databaseVersions = new List<DatabaseVersion>();

            var initialDbVersion = new DatabaseVersion(1);



            databaseVersions.Add(new Database("TestDb123"));


            return StartDatabaseInstaller(args, databaseVersions);
        }
    }

    public abstract class InstallationProgram
    {
        public static async Task StartDatabaseInstaller(string[] args, List<DatabaseVersion> databaseVersions)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: dotnet run [command] [connectionstring]");
                Console.ReadKey();

                return;
            }

            var installationHandler = BootstrapApplication(args[1]);

            switch (args[0].ToLower())
            {
                case "-i":
                    await installationHandler.Install(databaseVersions);
                    break;
                case "-p":
                    await installationHandler.PreviewDbChanges(databaseVersions);
                    break;
                case "-c":
                    installationHandler.AllDbChanges(databaseVersions);
                    break;
                case "-s":
                    await installationHandler.ShowCurrentInstallationState();
                    break;
                default:
                    Console.WriteLine("Valid command is required");
                    Console.ReadKey();
                    break;
            }

            Console.WriteLine($"Done");
            Console.ReadKey();
        }

        private static InstallationHandler BootstrapApplication(string connectionString)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging((builder) =>
            {
                builder.SetMinimumLevel(LogLevel.Information)
                        .AddFilter("Microsoft", LogLevel.Warning)
                        .AddFilter("System", LogLevel.Warning)
                        .AddConsole();
            });

            serviceCollection.AddTransient<InstallationHandler>();
            serviceCollection.AddDatabaseInstaller(connectionString);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceProvider.GetRequiredService<InstallationHandler>();
        }
    }
}
