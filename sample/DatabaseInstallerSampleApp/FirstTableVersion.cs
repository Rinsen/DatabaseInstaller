using System.Collections.Generic;
using Rinsen.DatabaseInstaller;
using Rinsen.DatabaseInstaller.Sql;
using System.Linq;
using System;

namespace DatabaseInstallerSampleApp
{
    public class FirstTableVersion : DatabaseVersion
    {
        public FirstTableVersion()
            : base(1)
        { }

        public override void AddTables(List<Table> tableCollection)
        {
            tableCollection.Add(GetMyFirstTable());
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
