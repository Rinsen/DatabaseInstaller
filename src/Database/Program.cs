using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Rinsen.DatabaseInstaller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Database
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Some arguments is needed");
                Console.ReadKey();
            }

            var installer = Initialize();

            switch (args[0].ToLower())
            {
                case "install":
                    Console.WriteLine("Installing...");
                    Install(installer);

                    break;
                case "preview":
                    PreviewDbChanges();
                    break;
                case "current":
                    ShowCurrentInstallationState(installer);
                    break;


                default:
                    Console.WriteLine("Some arguments is needed");
                    Console.ReadKey();
                    break;
            }

            Console.WriteLine("Done");
            Console.ReadKey();
        }

        private static void ShowCurrentInstallationState(Installer installer)
        {
            Console.WriteLine("Installed versions");
            Console.WriteLine("Id InstallationName InstalledVersion PreviousVersion StartedInstallatingVersion");
            foreach (var installationNameAndVersion in installer.GetVersionInformation())
            {
                Console.WriteLine($"{installationNameAndVersion.Id} {installationNameAndVersion.InstallationName} {installationNameAndVersion.InstalledVersion} {installationNameAndVersion.PreviousVersion} {installationNameAndVersion.StartedInstallingVersion}");
            }
        }

        private static void PreviewDbChanges()
        {
            var dbChanges = GetDbChangesToInstall();

            foreach (var installationName in dbChanges.Select(m => m.InstallationName).Distinct())
            {
                Console.WriteLine($"Installations for {installationName}");
                foreach (var dbChange in dbChanges.Where(dbc => dbc.InstallationName == installationName).OrderBy(m => m.Version))
                {
                    dbChange.InitializeUp();
                    Console.WriteLine($"Version {dbChange.Version}");
                    foreach (var command in dbChange.Commands)
                    {
                        Console.WriteLine(command);
                    }
                    Console.WriteLine();
                }
            } 
        }

        private static void Install(Installer installer)
        {
            var dbChangesToInstall = GetDbChangesToInstall();

            installer.Run(dbChangesToInstall);
        }



        private static Installer Initialize()
        {
            var configBuilder = new ConfigurationBuilder();

            configBuilder.AddUserSecrets<Program>();
            //http://bleedingnedge.com/2015/10/15/configuration-providers/ Maybe add more?!

            var config = configBuilder.Build();

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddDatabaseInstaller(config["Data:DefaultConnection:ConnectionString"]);
            serviceCollection.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>)));

            var serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceProvider.GetRequiredService<Installer>();
        }

        private static List<DatabaseVersion> GetDbChangesToInstall()
        {
            var assembly = Assembly.LoadFile("C:\\Users\\fredr\\Source\\Repos\\DatabaseInstaller\\sample\\DatabaseInstallerSampleApp\\bin\\Debug\\netcoreapp2.0\\DatabaseInstallerSampleApp.dll");

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
