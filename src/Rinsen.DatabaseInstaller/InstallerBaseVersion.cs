using System.Collections.Generic;
using Rinsen.DatabaseInstaller.SqlTypes;

namespace Rinsen.DatabaseInstaller
{
    public class InstallerBaseVersion : DatabaseVersion
    {
        private readonly string _tableName;

        public InstallerBaseVersion(string tableName)
            :base (1, "InstallerInstallation")
        {
            _tableName = tableName;
        }

        public override void AddDbChanges(List<IDbChange> tableCollection)
        {
            var table = new Table(_tableName);
            table.AddColumn("Id", new Int()).PrimaryKey().AutoIncrement();
            table.AddColumn("InstallationName", new NVarChar(1024)).Unique().NotNull();
            table.AddColumn("PreviousVersion", new Int()).NotNull();
            table.AddColumn("StartedInstallingVersion", new Int()).NotNull();
            table.AddColumn("InstalledVersion", new Int()).NotNull();

            tableCollection.Add(table);
        }
    }
}