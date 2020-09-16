using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Rinsen.DatabaseInstaller.ConsoleInstaller
{
    public class InstallationHandler
    {
        private readonly Installer _installer;
        private readonly ILogger<InstallationHandler> _logger;

        public InstallationHandler(Installer installer,
            ILogger<InstallationHandler> logger)
        {
            _installer = installer;
            _logger = logger;
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

        public async Task PreviewDbChanges(List<DatabaseVersion> databaseVersions)
        {
            var installationNamesAndVersion = await _installer.GetVersionInformationAsync();

            foreach (var installationName in databaseVersions.Select(m => m.InstallationName).Distinct())
            {
                Console.WriteLine($"Installations for {installationName}");
                Console.WriteLine();

                var installationNameAndVersion = installationNamesAndVersion.SingleOrDefault(vi => vi.InstallationName == installationName);
                var installationNameDbChanges = databaseVersions.Where(dbc => dbc.InstallationName == installationName);

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

        public void AllDbChanges(List<DatabaseVersion> databaseVersions)
        {
            foreach (var installationName in databaseVersions.Select(m => m.InstallationName).Distinct())
            {
                Console.WriteLine($"Installations for {installationName}");
                foreach (var dbChange in databaseVersions.Where(dbc => dbc.InstallationName == installationName).OrderBy(m => m.Version))
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

        public async Task Install(List<DatabaseVersion> databaseVersions)
        {
            await _installer.RunAsync(databaseVersions);
        }
    }
}
