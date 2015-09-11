using System.Collections.Generic;
using System.Data.SqlClient;

namespace Rinsen.DatabaseInstaller
{
    public interface IVersionStorage
    {
        bool IsInstalled();

        InstallationNameAndVersion Get(string name);

        IEnumerable<InstallationNameAndVersion> GetAll();

        void Create(InstallationNameAndVersion installedNameAndVersion);

        int StartInstallation(InstallationNameAndVersion installedVersion);

        int EndInstallation(InstallationNameAndVersion installedVersion);

        int UndoInstallation(InstallationNameAndVersion installedVersion);
    }
}
