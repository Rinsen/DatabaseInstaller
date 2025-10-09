using System.Collections.Generic;

namespace Rinsen.DatabaseInstaller.SqlTypes
{
    public class RawSql : IDbChange
    {
        public List<string> UpScripts { get; set; } = new List<string>();

        public List<string> DownScripts { get; set; } = new List<string>();

        public IReadOnlyList<string> GetDownScript(InstallerOptions installerOptions)
        {
            return DownScripts;
        }

        public IReadOnlyList<string> GetUpScript(InstallerOptions installerOptions)
        {
            return UpScripts;
        }
    }
}
