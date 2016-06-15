using System.Collections.Generic;
using System.Data.SqlClient;

namespace Rinsen.DatabaseInstaller
{
    public interface IVersionStorage
    {
        bool IsInstalled(SqlConnection connection, SqlTransaction transaction);

        InstallationNameAndVersion Get(string name, SqlConnection connection, SqlTransaction transaction);

        IEnumerable<InstallationNameAndVersion> GetAll(SqlConnection connection, SqlTransaction transaction);

        void Create(InstallationNameAndVersion installedNameAndVersion, SqlConnection connection, SqlTransaction transaction);

        int StartInstallation(InstallationNameAndVersion installedVersion, SqlConnection connection, SqlTransaction transaction);

        int EndInstallation(InstallationNameAndVersion installedVersion, SqlConnection connection, SqlTransaction transaction);
        
    }
}
