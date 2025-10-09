using System.Threading.Tasks;

namespace Rinsen.DatabaseInstaller
{
    public class InstallerHost
    {

        /// <summary>
        /// Creates a new instance of an installer host builder for configuring and constructing installer hosts.
        /// </summary>
        /// <returns>An <see cref="IInstallerHostBuilder"/> instance that can be used to configure and build an installer host.</returns>
        public static IInstallerHostBuilder CreateBuilder()
        {
            return new InstallerHostBuilder();
        }

        
    }
}
