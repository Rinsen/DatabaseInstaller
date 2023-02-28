using Microsoft.Extensions.Configuration;
using Rinsen.DatabaseInstaller;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InstallationSampleConsoleApp
{
    class Program
    {
        static Task Main(string[] args)
        {
            return InstallerHost.Start<InstallerStartup>();
        }
    }

    public class InstallerStartup : IInstallerStartup
    {
        public void DatabaseVersionsToInstall(List<DatabaseVersion> databaseVersions, IConfiguration configuration)
        {
            databaseVersions.Add(new SetDatabaseSettingsVersion(configuration));
            databaseVersions.Add(new CreateTables());
        }
    }
}
