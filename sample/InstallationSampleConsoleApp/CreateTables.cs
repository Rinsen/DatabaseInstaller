using System;
using System.Collections.Generic;
using Rinsen.DatabaseInstaller;

namespace InstallationSampleConsoleApp
{
    public class CreateTables : DatabaseVersion
    {
        public CreateTables() 
            : base(1)
        {
        }

        public override void AddDbChanges(List<IDbChange> dbChangeList)
        {
            throw new NotImplementedException();
        }
    }
}
