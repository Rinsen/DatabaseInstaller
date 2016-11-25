using Rinsen.DatabaseInstaller;
using System.Collections.Generic;

namespace DatabaseInstallerSampleApp
{
    public class ThirdVersion : DatabaseVersion
    {
        public class MyItem
        {
            public int IdColumn { get; set; }

            public string Data { get; set; }
        }

        public ThirdVersion()
            : base (3)
        { }

        public override void AddDbChanges(List<IDbChange> dbChangeList)
        {
            var table = dbChangeList.AddNewTable<MyItem>();
            table.AddColumn(m => m.IdColumn);
            table.AddColumn(m => m.Data);

            var index = dbChangeList.AddNewUniqueClusteredIndex<MyItem>("MyItemIndex");
            index.AddColumn(m => m.IdColumn);
        }
    }
}
