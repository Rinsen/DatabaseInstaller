using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Rinsen.DatabaseInstaller
{
    public interface IDatabaseSetup
    {
        void DatabaseVersionsToInstall(List<DatabaseVersion> databaseVersions, IConfiguration configuration);
    }
}
