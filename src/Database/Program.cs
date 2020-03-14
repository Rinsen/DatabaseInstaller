using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Rinsen.DatabaseInstaller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Database
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Some arguments is needed");
                Console.ReadKey();
            }

            var installer = CreateInstaller();

            switch (args[0].ToLower())
            {
                case "install":
                    Console.WriteLine("Installing...");
                    await Install(installer);
                    break;
                case "preview":
                    await PreviewDbChanges(installer);
                    break;
                case "complete":
                    AllDbChanges();
                    break;
                case "current":
                    await ShowCurrentInstallationState(installer);
                    break;
                default:
                    Console.WriteLine("Some arguments is needed");
                    Console.ReadKey();
                    break;
            }

            Console.WriteLine($"Done with {args[0]}");
            Console.ReadKey();
        }

        private static async Task ShowCurrentInstallationState(Installer installer)
        {
            Console.WriteLine("Installed versions");
            Console.WriteLine("Id InstallationName InstalledVersion PreviousVersion StartedInstallatingVersion");
            foreach (var installationNameAndVersion in await installer.GetVersionInformation())
            {
                Console.WriteLine($"{installationNameAndVersion.Id} {installationNameAndVersion.InstallationName} {installationNameAndVersion.InstalledVersion} {installationNameAndVersion.PreviousVersion} {installationNameAndVersion.StartedInstallingVersion}");
            }
        }

        private static async Task PreviewDbChanges(Installer installer)
        {
            var dbChanges = GetAllDbChanges();

            var installationNamesAndVersion = await installer.GetVersionInformation();

            foreach (var installationName in dbChanges.Select(m => m.InstallationName).Distinct())
            {
                Console.WriteLine($"Installations for {installationName}");
                Console.WriteLine();

                var installationNameAndVersion = installationNamesAndVersion.SingleOrDefault(vi => vi.InstallationName == installationName);
                var installationNameDbChanges = dbChanges.Where(dbc => dbc.InstallationName == installationName);

                if (installationNameAndVersion != default(InstallationNameAndVersion))
                {
                    Console.WriteLine($"Version {installationNameAndVersion.InstalledVersion} of {installationNameDbChanges.Max(m => m.Version)} installed");

                    foreach (var dbChange in installationNameDbChanges.Where(dbc => dbc.Version > installationNameAndVersion.InstalledVersion)
                    .OrderBy(m => m.Version))
                    {
                        PrintDbChange(dbChange);
                    }
                }
                else
                {
                    Console.WriteLine($"No previous version installed");

                    foreach (var dbChange in installationNameDbChanges.OrderBy(m => m.Version))
                    {
                        PrintDbChange(dbChange);
                    }
                }
                Console.WriteLine("----------------------------------------------------------");
                Console.WriteLine();
            }
        }

        private static void PrintDbChange(DatabaseVersion dbChange)
        {
            Console.WriteLine($"Database change {dbChange.GetType().Name} version {dbChange.Version} ");
            foreach (var command in dbChange.UpCommands)
            {
                Console.WriteLine(command);
            }
            Console.WriteLine();
        }

        private static void AllDbChanges()
        {
            var dbChanges = GetAllDbChanges();

            foreach (var installationName in dbChanges.Select(m => m.InstallationName).Distinct())
            {
                Console.WriteLine($"Installations for {installationName}");
                foreach (var dbChange in dbChanges.Where(dbc => dbc.InstallationName == installationName).OrderBy(m => m.Version))
                {
                    Console.WriteLine($"Version {dbChange.Version}");
                    foreach (var command in dbChange.UpCommands)
                    {
                        Console.WriteLine(command);
                    }
                    Console.WriteLine();
                }
            }
        }

        private static async Task Install(Installer installer)
        {
            var dbChangesToInstall = GetAllDbChanges();

            await installer.RunAsync(dbChangesToInstall);
        }

        private static Installer CreateInstaller()
        {
            var configBuilder = new ConfigurationBuilder();

            configBuilder.AddUserSecrets<Program>();
            //http://bleedingnedge.com/2015/10/15/configuration-providers/ Maybe add more or less?! Only args support? How to find assembly?

            var config = configBuilder.Build();

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddDatabaseInstaller(config["Data:DefaultConnection:ConnectionString"]);
            serviceCollection.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>)));

            var serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceProvider.GetRequiredService<Installer>();
        }

        private static List<DatabaseVersion> GetAllDbChanges()
        {
            var assembly = Assembly.LoadFile("C:\\Users\\fredr\\Source\\Repos\\DatabaseInstaller\\sample\\DatabaseInstallerSampleApp\\bin\\Debug\\netcoreapp2.0\\DatabaseInstallerSampleApp.dll"); // This is...

            var dbChanges = new List<DatabaseVersion>();

            foreach (var dbChange in assembly.ExportedTypes.Where(t => t.IsSubclassOf(typeof(DatabaseVersion))))
            {
                dbChanges.Add((DatabaseVersion)Activator.CreateInstance(dbChange));
            }

            return dbChanges;
        }
    }

    class Logger<T> : ILogger<T>
    {
        public Logger()
        {

        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new NoScope();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            
        }

        private class NoScope : IDisposable
        {
            public void Dispose()
            {
                
            }
        }
    }
}
