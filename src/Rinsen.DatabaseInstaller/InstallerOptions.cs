namespace Rinsen.DatabaseInstaller
{
    public class InstallerOptions
    {
        /// <summary>
        /// The database name to create or update.
        /// </summary>
        public string DatabaseName { get; }

        /// <summary>
        /// The database schema name to create or update.
        /// </summary>
        public string Schema { get; }

        /// <summary>
        /// The connection string to the database server.
        /// </summary>
        public string ConnectionString { get; }

        /// <summary>
        /// The connection string name in configuration to use for the database server.
        /// </summary>
        public string ConnectionStringName { get; }

        public InstallerOptions(string databaseName, string schema, string connectionString, string connectionStringName)
        {
            DatabaseName = databaseName;
            Schema = schema;
            ConnectionString = connectionString;
            ConnectionStringName = connectionStringName;
        }
    }
}
