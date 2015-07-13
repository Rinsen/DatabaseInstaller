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

        public override void AddTables(List<Table> tableCollection)
        {
            tableCollection.Add(GetMySecondTable());
            tableCollection.Add(GetMyFirstTable());
        }

        private Table GetMySecondTable()
        {
            var table = new Table("MySecondTable");
            table.AddAutoIncrementColumn("Id");
            table.AddNVarCharColumn("SomeInfo", 100).NotNull();
            return table;
        }

        private Table GetMyFirstTable()
        {
            var table = new Table("MyFirstTable");
            table.AddAutoIncrementColumn("Id");
            table.AddNVarCharColumn("SomeInfo", 100).NotNull();
            return table;
        }
    }
}
