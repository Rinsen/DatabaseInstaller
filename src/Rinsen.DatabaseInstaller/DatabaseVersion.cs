using Rinsen.DatabaseInstaller.Sql;
using System.Collections.Generic;

namespace Rinsen.DatabaseInstaller
{
    public abstract class DatabaseVersion
    {
        /// <summary>
        /// Database version description
        /// </summary>
        /// <param name="version">Version number</param>
        /// <param name="installationName">Installation name, if none specified the default will be this class namespace</param>
        public DatabaseVersion(int version, string installationName = null)
        {
            if (string.IsNullOrEmpty(installationName))
            {
                InstallationName = GetType().Namespace;
            }
            else
            {
                InstallationName = installationName;
            }
            Version = version;
        }

        private List<Table> _tables;

        public string InstallationName { get; private set; }
        public int Version { get; private set; }
        internal List<string> Commands { get; private set; }
        
        internal void SetTables(List<Table> tableCollection)
        {
            if (_tables == null)
            {
                _tables = tableCollection;
            }
        }

        public virtual void AddTables(List<Table> tableCollection)
        {

        }

        internal void PrepareUp()
        {
            Commands.Clear();
            Up();
            foreach (var table in _tables)
            {
                Commands.Add(table.GetUpScript());
            }
        }

        internal void PrepareDown()
        {

        }

        public virtual void Up()
        {

        }

        public virtual void Down()
        {

        }

        public void AddSql(string sql)
        {
            Commands.Add(sql);
        }
    }
}
