namespace Rinsen.DatabaseInstaller
{
    public class InstallerOptions
    {
        /// <summary>
        /// The database name to create or update.
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// The database schema name to create or update.
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// The connection string to the database server.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// The connection string name in configuration to use for the database server.
        /// </summary>
        public string ConnectionStringName { get; internal set; }
    }
}
