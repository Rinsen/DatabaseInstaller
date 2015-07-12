namespace Rinsen.DatabaseInstaller
{
    public class InstallerOptions
    {
        public InstallerOptions()
        {
            InstalledVersionsDatabaseTableName = "InstalledVersions";
        }

        public string ConnectionString { get; set; }

        public string InstalledVersionsDatabaseTableName { get; set; }
    }
}
