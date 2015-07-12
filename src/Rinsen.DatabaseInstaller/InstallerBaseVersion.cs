using Rinsen.DatabaseInstaller.Sql;

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

        public override void Up()
        {
            var table = new Table(_tableName);
            table.AddColumn("Id", new Int()).PrimaryKey().AutoIncrement();
            table.AddColumn("InstallationName", new NVarChar(1024)).Unique().NotNull();
            table.AddColumn("PreviousVersion", new Int()).NotNull();
            table.AddColumn("StartedInstallingVersion", new Int()).NotNull();
            table.AddColumn("InstalledVersion", new Int()).NotNull();
            var createScript = table.GetUpScript();

            AddSql(createScript);
        }
    }
}