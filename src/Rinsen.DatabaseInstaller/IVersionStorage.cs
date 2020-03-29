using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Rinsen.DatabaseInstaller
{
    public interface IVersionStorage
    {
        Task<bool> IsInstalled(SqlConnection connection);

        Task<bool> IsInstalled(SqlConnection connection, SqlTransaction transaction);

        InstallationNameAndVersion Get(string name, SqlConnection connection, SqlTransaction transaction);

        Task<InstallationNameAndVersion> GetAsync(string name, SqlConnection connection, SqlTransaction transaction);

        Task<IEnumerable<InstallationNameAndVersion>> GetAll(SqlConnection connection, SqlTransaction transaction);

        Task Create(InstallationNameAndVersion installedNameAndVersion, SqlConnection connection, SqlTransaction transaction);

        Task<int> StartInstallation(InstallationNameAndVersion installedVersion, SqlConnection connection, SqlTransaction transaction);

        Task<int> EndInstallationAsync(InstallationNameAndVersion installedVersion, SqlConnection connection, SqlTransaction transaction);

        int EndInstallation(InstallationNameAndVersion installedVersion, SqlConnection connection, SqlTransaction transaction);

    }
}
