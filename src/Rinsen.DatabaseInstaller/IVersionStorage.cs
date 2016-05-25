using System.Collections.Generic;
using System.Data.SqlClient;

namespace Rinsen.DatabaseInstaller
{
    public interface IVersionStorage
    {
        bool IsInstalled();

        InstallationNameAndVersion Get(string name, SqlConnection connection);

        IEnumerable<InstallationNameAndVersion> GetAll(SqlConnection connection);

        void Create(InstallationNameAndVersion installedNameAndVersion, SqlConnection connection, SqlTransaction transaction);

        int StartInstallation(InstallationNameAndVersion installedVersion, SqlConnection connection, SqlTransaction transaction);

        int EndInstallation(InstallationNameAndVersion installedVersion, SqlConnection connection, SqlTransaction transaction);
        
    }
}
