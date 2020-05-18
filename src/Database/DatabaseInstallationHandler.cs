using Rinsen.DatabaseInstaller;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    public class DatabaseInstallationHandler
    {
        private readonly Installer _installer;
        private readonly DatabaseInstallerOptions _databaseInstallerOptions;

        public DatabaseInstallationHandler(Installer installer,
            DatabaseInstallerOptions databaseInstallerOptions)
        {
            _installer = installer;
            _databaseInstallerOptions = databaseInstallerOptions;
        }

        public async Task ShowCurrentInstallationState()
        {
            Console.WriteLine("Installed versions");
            Console.WriteLine("Id InstallationName InstalledVersion PreviousVersion StartedInstallatingVersion");
            foreach (var installationNameAndVersion in await _installer.GetVersionInformationAsync())
            {
                Console.WriteLine($"{installationNameAndVersion.Id} {installationNameAndVersion.InstallationName} {installationNameAndVersion.InstalledVersion} {installationNameAndVersion.PreviousVersion} {installationNameAndVersion.StartedInstallingVersion}");
            }
        }

        public async Task PreviewDbChanges()
        {
            var dbChanges = GetAllDbChanges();

            var installationNamesAndVersion = await _installer.GetVersionInformationAsync();

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

        public void AllDbChanges()
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

        public async Task Install()
        {
            var dbChangesToInstall = GetAllDbChanges();

            await _installer.RunAsync(dbChangesToInstall);
        }

        private List<DatabaseVersion> GetAllDbChanges()
        {
            Assembly assembly = null;
            foreach (var file in Directory.EnumerateFiles(_databaseInstallerOptions.Path).Where(f => f.EndsWith(".dll") && !f.EndsWith("Rinsen.DatabaseInstaller.dll")))
            {
                if (file.EndsWith(_databaseInstallerOptions.AssemblyName))
                {
                    assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(file);
                }
                else
                {
                    AssemblyLoadContext.Default.LoadFromAssemblyPath(file);
                }
            }

            if (!(assembly is object))
                throw new Exception($"Installation assembly {_databaseInstallerOptions.AssemblyName} not found at path {_databaseInstallerOptions.Path}");

            var dbChanges = new List<DatabaseVersion>();
            foreach (var dbChange in assembly.ExportedTypes.Where(t => t.IsSubclassOf(typeof(DatabaseVersion))))
            {
                dbChanges.Add((DatabaseVersion)Activator.CreateInstance(dbChange));
            }

            return dbChanges;
        }
    }
}
