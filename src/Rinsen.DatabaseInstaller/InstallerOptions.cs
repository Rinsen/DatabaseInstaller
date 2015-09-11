namespace Rinsen.DatabaseInstaller
{
    public class InstallerOptions
    {
        public string ConnectionString { get; set; }

        public string InstalledVersionsDatabaseTableName { get { return "InstalledVersions"; } }
    }
}
