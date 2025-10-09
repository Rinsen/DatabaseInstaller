using System;
using System.Collections.Generic;
using System.Text;

namespace Rinsen.DatabaseInstaller.Tests
{
    public static class TestHelper
    {
        public static InstallerOptions GetInstallerOptions()
        {
            return new InstallerOptions("TestDb", "dbo", string.Empty, string.Empty);
        }
    }
}
