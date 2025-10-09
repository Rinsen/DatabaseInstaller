using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Rinsen.DatabaseInstaller
{
    public interface IInstallerHostBuilder
    {
        /// <summary>
        /// Registers a database setup of the specified type for use in the installer host.
        /// </summary>
        /// <remarks>Use this method to add custom database setup logic by providing a type that
        /// implements <see cref="IDatabaseSetup"/>. Only one type can be registered.</remarks>
        /// <typeparam name="T">The type of database setup to register. Must be a non-abstract class that implements <see
        /// cref="IDatabaseSetup"/>.</typeparam>
        void AddDatabaseSetup<T>() where T : class, IDatabaseSetup, new();

        /// <summary>
        /// Registers a data seed of the specified type for initialization during application startup.
        /// </summary>
        /// <remarks>Use this method to add custom data seeding logic to the application's startup
        /// process.</remarks>
        /// <typeparam name="T">The type of data seed to register. Must be a non-abstract class that implements <see cref="IDataSeed"/>.</typeparam>
        void AddDataSeed<T>() where T : class, IDataSeed;

        /// <summary>
        /// Configures additional services by invoking the specified delegate on the service collection.
        /// </summary>
        /// <remarks>Use this method to register custom services or modify existing service registrations
        /// before the service provider is built. The delegate is called with the current service collection, allowing
        /// for flexible configuration.</remarks>
        /// <param name="serviceCollection">A delegate that receives the <see cref="IServiceCollection"/> to which services can be added or configured.</param>
        void AddServices(Action<IServiceCollection> serviceCollection);

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
        /// <returns>Task.</returns>
        Task Start();
    }

    public class InstallerHostBuilder : IInstallerHostBuilder
    {
        public InstallerHostBuilder()
        {
        }


        public void AddServices(Action<IServiceCollection> serviceCollection)
        {
            InstallationProgram.AddServices(serviceCollection);
        }

        /// <inheritdoc/>>
        public Task Start()
        {
            return InstallationProgram.StartDatabaseInstaller();
        }

        public void AddDatabaseSetup<T>() where T : class, IDatabaseSetup, new()
        {
            InstallationProgram.AddDatabaseSetup<T>();
        }

        public void AddDataSeed<T>() where T : class, IDataSeed        
        {
            InstallationProgram.AddDataSeed<T>();
        }
    }
}
