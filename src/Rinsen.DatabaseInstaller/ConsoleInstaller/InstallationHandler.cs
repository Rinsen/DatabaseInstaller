using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rinsen.DatabaseInstaller.ConsoleInstaller
{
    internal class InstallationHandler
    {
        private readonly Installer _installer;
        private readonly InstallerOptions _installerOptions;
        private readonly ILogger<InstallationHandler> _logger;

        public InstallationHandler(Installer installer,
            InstallerOptions installerOptions,
            ILogger<InstallationHandler> logger)
        {
            _installer = installer;
            _installerOptions = installerOptions;
            _logger = logger;
        }

        public async Task ShowCurrentInstallationState()
        {
            _logger.LogInformation("Installed versions");
            _logger.LogInformation("Id InstallationName InstalledVersion PreviousVersion StartedInstallatingVersion");
            foreach (var installationNameAndVersion in await _installer.GetVersionInformationAsync())
            {
                _logger.LogInformation($"{installationNameAndVersion.Id} {installationNameAndVersion.InstallationName} {installationNameAndVersion.InstalledVersion} {installationNameAndVersion.PreviousVersion} {installationNameAndVersion.StartedInstallingVersion}");
            }
        }

        public async Task PreviewDbChanges(List<DatabaseVersion> databaseVersions)
        {
            var installationNamesAndVersion = await _installer.GetVersionInformationAsync();

            foreach (var installationName in databaseVersions.Select(m => m.InstallationName).Distinct())
            {
                _logger.LogInformation($"Installations for {installationName}");

                var installationNameAndVersion = installationNamesAndVersion.SingleOrDefault(vi => vi.InstallationName == installationName);
                var installationNameDbChanges = databaseVersions.Where(dbc => dbc.InstallationName == installationName);

                if (installationNameAndVersion != default(InstallationNameAndVersion))
                {
                    _logger.LogInformation($"Version {installationNameAndVersion.InstalledVersion} of {installationNameDbChanges.Max(m => m.Version)} installed");

                    foreach (var dbChange in installationNameDbChanges.Where(dbc => dbc.Version > installationNameAndVersion.InstalledVersion)
                    .OrderBy(m => m.Version))
                    {
                        PrintDbChange(dbChange);
                    }
                }
                else
                {
                    _logger.LogInformation($"No previous version installed");

                    foreach (var dbChange in installationNameDbChanges.OrderBy(m => m.Version))
                    {
                        PrintDbChange(dbChange);
                    }
                }
                _logger.LogInformation("----------------------------------------------------------");
            }
        }

        private void PrintDbChange(DatabaseVersion dbChange)
        {
            _logger.LogInformation($"Database change {dbChange.GetType().Name} version {dbChange.Version} ");
            foreach (var command in dbChange.GetUpCommands(_installerOptions))
            {
                _logger.LogInformation(command);
            }
        }

        public void AllDbChanges(List<DatabaseVersion> databaseVersions)
        {
            foreach (var installationName in databaseVersions.Select(m => m.InstallationName).Distinct())
            {
                _logger.LogInformation($"Installations for {installationName}");
                foreach (var dbChange in databaseVersions.Where(dbc => dbc.InstallationName == installationName).OrderBy(m => m.Version))
                {
                    _logger.LogInformation($"Version {dbChange.Version}");
                    foreach (var command in dbChange.GetUpCommands(_installerOptions))
                    {
                        _logger.LogInformation(command);
                    }
                }
            }
        }

        public async Task Install(List<DatabaseVersion> databaseVersions)
        {
            await _installer.RunAsync(databaseVersions);
        }
    }
}
