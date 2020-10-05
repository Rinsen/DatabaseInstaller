using System.Collections.Generic;

namespace Rinsen.DatabaseInstaller
{
    public interface IInstallerStartup
    {
        void DatabaseVersionsToInstall(List<DatabaseSettingsVersion> databaseVersions);
    }
}
