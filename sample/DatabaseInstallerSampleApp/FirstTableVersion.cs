using System.Collections.Generic;
using Rinsen.DatabaseInstaller;

namespace DatabaseInstallerSampleApp
{
    public class FirstTableVersion : DatabaseVersion
    {
        public FirstTableVersion()
            : base(1)
        { }

        public override void AddDbChanges(List<IDbChange> dbChangeList)
        {
            var myFirstTable = dbChangeList.AddNewTable("MyFirstTable");
            myFirstTable.AddAutoIncrementColumn("Id");
            myFirstTable.AddNVarCharColumn("SomeInfo", 100).NotNull();
        }
    }
}
