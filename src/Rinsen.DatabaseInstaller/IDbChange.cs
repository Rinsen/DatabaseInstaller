using System.Collections.Generic;

namespace Rinsen.DatabaseInstaller
{
    public interface IDbChange
    {
        List<string> GetUpScript();

        List<string> GetDownScript();

    }
}
