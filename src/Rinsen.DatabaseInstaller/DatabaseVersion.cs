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

        private List<IDbChange> _dbChangeList;

        public string InstallationName { get; private set; }
        public int Version { get; private set; }
        internal List<string> Commands { get; private set; } = new List<string>();
        
        internal void SetTables(List<IDbChange> dbChangeList)
        {
            if (_dbChangeList == null)
            {
                _dbChangeList = dbChangeList;
            }
        }

        public virtual void AddDbChanges(List<IDbChange> dbChangeList)
        {

        }

        internal void PrepareUp()
        {
            Commands.Clear();
            foreach (var dbChange in _dbChangeList)
            {
                Commands.Add(dbChange.GetUpScript());
            }
        }

        internal void PrepareDown()
        {
            Commands.Clear();
            foreach (var dbChange in _dbChangeList)
            {
                Commands.Add(dbChange.GetDownScript());
            }
        }
    }
}
