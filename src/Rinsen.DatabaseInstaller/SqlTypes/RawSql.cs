using System.Collections.Generic;

namespace Rinsen.DatabaseInstaller.SqlTypes
{
    public class RawSql : IDbChange
    {
        public List<string> UpScripts { get; set; }

        public List<string> DownScripts { get; set; }

        public List<string> GetDownScript()
        {
            return DownScripts;
        }

        public List<string> GetUpScript()
        {
            return UpScripts;
        }
    }
}
