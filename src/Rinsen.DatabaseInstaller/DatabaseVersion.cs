using System;
using System.Collections.Generic;
using System.Linq;

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

        public string InstallationName { get; }

        public int Version { get; }

        public Database Database 
        { 
            get 
            {
                var dbChangeList = new List<IDbChange>();

                AddDbChanges(dbChangeList);

                return (Database)dbChangeList.SingleOrDefault(m => m is Database);
            } 
        }

        public IEnumerable<string> UpCommands
        {
            get
            {
                var dbChangeList = new List<IDbChange>();

                AddDbChanges(dbChangeList);

                var commands = new List<string>();

                foreach (var dbChange in dbChangeList)
                {
                    commands.AddRange(dbChange.GetUpScript());
                }

                return commands;
            }
        }

        public IEnumerable<string> DownCommands
        {
            get
            {
                throw new NotImplementedException();

                //var dbChangeList = new List<IDbChange>();

                //AddDbChanges(dbChangeList);

                //var commands = new List<string>();

                //foreach (var dbChange in dbChangeList)
                //{
                //    commands.AddRange(dbChange.GetDownScript());
                //}

                //return commands;
            }
        }

        public abstract void AddDbChanges(List<IDbChange> dbChangeList);
    }
}
