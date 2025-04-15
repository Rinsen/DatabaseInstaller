using System.Threading.Tasks;

namespace Rinsen.DatabaseInstaller
{
    public class InstallerHost
    {
        /// <summary>
        /// Database installer host.
        /// 
        /// This can be used to create databases, db users and schemas from c# fluent code definitions.
        /// <para>
        /// Requires configuration for the following settings to work:
        /// * Command: Install, Preview, ShowAll, CurrentState
        /// * DatabaseName: Name of the database to install.
        /// * Schema: Name of the schema to install.
        /// * ConnectionString: Connection string to the database server.
        /// </para>
        /// </summary>
        /// <typeparam name="T">Installation assembly type</typeparam>
        /// <returns>Task.</returns>
        public static Task Start<T>() where T : class, IInstallerStartup, new()
        {
            return InstallationProgram.StartDatabaseInstaller<T>();
        }
    }
}
