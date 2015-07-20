using System.Collections.Generic;
using Rinsen.DatabaseInstaller;
using Rinsen.DatabaseInstaller.Sql;

namespace DatabaseInstallerSampleApp
{
    public class SecondTableVersion : DatabaseVersion
    {
        public SecondTableVersion()
            : base(2)
        { }

        public override void AddDbChanges(List<IDbChange> dbChangeList)
        {
            var mySecondTable = dbChangeList.AddNewTable("MySecondTable");
            mySecondTable.AddAutoIncrementColumn("Id");
            mySecondTable.AddNVarCharColumn("SomeInfo", 100).NotNull();

            var myFirstTable = dbChangeList.AddNewTable("MyFirstTable");
            myFirstTable.AddAutoIncrementColumn("Id");
            myFirstTable.AddNVarCharColumn("SomeInfo", 100).NotNull();
        }
        
    }
}
