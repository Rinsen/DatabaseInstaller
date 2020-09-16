using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Database
{
    public class DatabaseInstallerHostedService : IHostedService
    {
        private readonly DatabaseInstallerOptions _databaseInstallerOptions;
        private readonly DatabaseInstallationHandler _databaseInstallationHandler;

        public DatabaseInstallerHostedService(DatabaseInstallerOptions databaseInstallerOptions, 
            DatabaseInstallationHandler databaseInstallationHandler)
        {
            _databaseInstallerOptions = databaseInstallerOptions;
            _databaseInstallationHandler = databaseInstallationHandler;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            switch (_databaseInstallerOptions.Command)
            {
                case "install":
                    Console.WriteLine("Installing...");
                    await _databaseInstallationHandler.Install();
                    break;
                case "preview":
                    await _databaseInstallationHandler.PreviewDbChanges();
                    break;
                case "complete":
                    _databaseInstallationHandler.AllDbChanges();
                    break;
                case "current":
                    await _databaseInstallationHandler.ShowCurrentInstallationState();
                    break;
                default:
                    Console.WriteLine("Some command is needed");
                    Console.ReadKey();
                    break;
            }
            Console.WriteLine($"Done with {_databaseInstallerOptions.Command}");
            Console.ReadKey();
        }

            public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
