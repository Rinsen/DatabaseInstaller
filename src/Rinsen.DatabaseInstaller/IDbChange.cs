using System.Collections.Generic;

namespace Rinsen.DatabaseInstaller
{
    public interface IDbChange
    {
        IReadOnlyList<string> GetUpScript(InstallerOptions installerOptions);

        IReadOnlyList<string> GetDownScript(InstallerOptions installerOptions);

    }
}
