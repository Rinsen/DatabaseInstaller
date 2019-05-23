using Rinsen.DatabaseInstaller;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseInstallerSampleApp.Test
{
    public class TestVersion : DatabaseVersion
    {
        public TestVersion()
            :base(1)
        {

        }

        public override void AddDbChanges(List<IDbChange> dbChangeList)
        {
            var testTable = dbChangeList.AddNewTable("TestTable123");

            testTable.AddDecimalColumn("Column", 10, 2).ForeignKey("MyMissingTable");
        }
    }
}
