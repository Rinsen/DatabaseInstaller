namespace Rinsen.DatabaseInstaller
{
    public class InstallationNameAndVersion
    {
        public int Id { get; set; }

        public string InstallationName { get; set; } = string.Empty;

        public int PreviousVersion { get; set; }

        public int StartedInstallingVersion { get; set; }

        public int InstalledVersion { get; set; }
    }
}
