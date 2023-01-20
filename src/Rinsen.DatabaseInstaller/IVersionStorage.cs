using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace Rinsen.DatabaseInstaller
{
    public interface IVersionStorage
    {
        Task<bool> IsInstalledAsync(SqlConnection connection);

        Task<bool> IsInstalledAsync(SqlConnection connection, SqlTransaction transaction);

        Task<InstallationNameAndVersion> GetAsync(string name, SqlConnection connection, SqlTransaction transaction);

        Task<IEnumerable<InstallationNameAndVersion>> GetAllAsync(SqlConnection connection, SqlTransaction transaction);

        Task CreateAsync(InstallationNameAndVersion installedNameAndVersion, SqlConnection connection, SqlTransaction transaction);

        Task<int> StartInstallationAsync(InstallationNameAndVersion installedVersion, SqlConnection connection, SqlTransaction transaction);

        Task<int> EndInstallationAsync(InstallationNameAndVersion installedVersion, SqlConnection connection, SqlTransaction transaction);

    }   
}
