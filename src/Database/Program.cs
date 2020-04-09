using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Rinsen.DatabaseInstaller;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
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
            var path = @"C:\Users\fredr\Source\Repos\WebSite\src\Rinsen.HomeControl.Installation\bin\Debug\netcoreapp3.1\publish";
            //var path = @"C:\Users\fredr\Source\Repos\InnovationBoost\src\Rinsen.InnovationBoost.Installation\bin\Debug\netcoreapp3.1\publish";
            var installationAssemblyName = "Rinsen.HomeControl.Installation.dll";
            var installHandler = new InstallHandler();
            var installer = installHandler.CreateInstaller();

            switch (args[0].ToLower())
            {
                case "install":
                    Console.WriteLine("Installing...");
                    await installHandler.Install(installer, path, installationAssemblyName);
                    break;
                case "preview":
                    await installHandler.PreviewDbChanges(installer, path, installationAssemblyName);
                    break;
                case "complete":
                    installHandler.AllDbChanges(path, installationAssemblyName);
                    break;
                case "current":
                    await installHandler.ShowCurrentInstallationState(installer);
                    break;
                default:
                    Console.WriteLine("Some arguments is needed");
                    Console.ReadKey();
                    break;
            }

            Console.WriteLine($"Done with {args[0]}");
            Console.ReadKey();
        }

    }

    public class InstallHandler 
    { 

        public async Task ShowCurrentInstallationState(Installer installer)
        {
            Console.WriteLine("Installed versions");
            Console.WriteLine("Id InstallationName InstalledVersion PreviousVersion StartedInstallatingVersion");
            foreach (var installationNameAndVersion in await installer.GetVersionInformationAsync())
            {
                Console.WriteLine($"{installationNameAndVersion.Id} {installationNameAndVersion.InstallationName} {installationNameAndVersion.InstalledVersion} {installationNameAndVersion.PreviousVersion} {installationNameAndVersion.StartedInstallingVersion}");
            }
        }

        public async Task PreviewDbChanges(Installer installer, string path, string installationAssemblyName)
        {
            var dbChanges = GetAllDbChanges(path, installationAssemblyName);

            var installationNamesAndVersion = await installer.GetVersionInformationAsync();

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

        public void AllDbChanges(string path, string installationAssemblyName)
        {
            var dbChanges = GetAllDbChanges(path, installationAssemblyName);

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

        public async Task Install(Installer installer, string path, string installationAssemblyName)
        {
            var dbChangesToInstall = GetAllDbChanges(path, installationAssemblyName);

            await installer.RunAsync(dbChangesToInstall);
        }

        public Installer CreateInstaller()
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

        private List<DatabaseVersion> GetAllDbChanges(string path, string installationAssemblyName)
        {
            var loadedAssemblies =  GetType().Assembly.GetReferencedAssemblies();
            Assembly assembly = null;
            foreach (var file in Directory.EnumerateFiles(path).Where(f => f.EndsWith(".dll") && !f.EndsWith("Rinsen.DatabaseInstaller.dll")))
            {
                if (file.EndsWith(installationAssemblyName))
                {
                    assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(file);
                }
                else
                {
                    var assemblyFile = Assembly.LoadFile(file);
                    
                    if (!loadedAssemblies.Any(la => la.FullName == assemblyFile.FullName))
                    {
                        try
                        {
                            AssemblyLoadContext.Default.LoadFromAssemblyPath(file);
                        }
                        catch (Exception)
                        {

                        }
                        
                    }
                }
            }

            if (!(assembly is object))
                throw new Exception($"Installation assembly {installationAssemblyName} not found at path {path}");

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
