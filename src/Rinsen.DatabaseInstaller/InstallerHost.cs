using System.Threading.Tasks;

namespace Rinsen.DatabaseInstaller
{
    public class InstallerHost
    {
        public static Task Start<T>() where T : class, IInstallerStartup, new()
        {
            return new InstallationProgram().StartDatabaseInstaller<T>();
        }
    }
}
