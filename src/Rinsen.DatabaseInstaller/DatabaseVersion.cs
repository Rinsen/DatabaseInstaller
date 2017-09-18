using Rinsen.DatabaseInstaller.SqlTypes;
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

        private List<IDbChange> _dbChangeList = new List<IDbChange>();

        public string InstallationName { get; }
        public int Version { get; }
        public IEnumerable<string> Commands { get { return _commands; } }
        private List<string> _commands = new List<string>();


        public void InitializeUp()
        {
            AddDbChanges(_dbChangeList);
            PrepareUp();
        }
        
        public virtual void AddDbChanges(List<IDbChange> dbChangeList)
        {

        }

        private void PrepareUp()
        {
            _commands.Clear();
            foreach (var dbChange in _dbChangeList)
            {
                _commands.AddRange(dbChange.GetUpScript());
            }
        }

        private void PrepareDown()
        {
            _commands.Clear();
            foreach (var dbChange in _dbChangeList)
            {
                _commands.AddRange(dbChange.GetDownScript());
            }
        }
    }
}
