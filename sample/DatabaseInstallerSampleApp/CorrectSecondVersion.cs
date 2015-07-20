using Rinsen.DatabaseInstaller;
using Rinsen.DatabaseInstaller.Sql;
using System.Collections.Generic;

namespace DatabaseInstallerSampleApp
{
    public class CorrectSecondVersion : DatabaseVersion
    {
        public CorrectSecondVersion()
            : base (2)
        { }

        public override void AddDbChanges(List<IDbChange> dbChangeList)
        {
            var tableAlteration = dbChangeList.AddNewTableAlteration("MyFirstTable");
            tableAlteration.AddBitColumn("BitColumn");
            tableAlteration.AddDateTimeOffsetColumn("Created").NotNull();
            tableAlteration.DeleteColumn("SomeInfo");
        }
    }
}
