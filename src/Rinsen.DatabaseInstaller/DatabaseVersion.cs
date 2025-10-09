using System;
using System.Collections.Generic;

namespace Rinsen.DatabaseInstaller
{
    public abstract class DatabaseVersion
    {

        /// <summary>
        /// Initializes a new instance of the DatabaseVersion class with the specified version number. This will user the class namespace as installation name.
        /// </summary>
        /// <param name="version">The version number to assign to the database version. Must be a non-negative integer.</param>
        protected DatabaseVersion(int version)
            : this(version, null)
        {
                
        }


        /// <summary>
        /// Database version description
        /// </summary>
        /// <param name="version">Version number</param>
        /// <param name="installationName">Installation name, if none specified the default will be this class namespace</param>
        public DatabaseVersion(int version, string? installationName)
        {
            if (string.IsNullOrEmpty(installationName))
            {
                InstallationName = GetType().Namespace ?? throw new ArgumentException("Installation name cannot be null or empty");
            }
            else
            {   
                InstallationName = installationName;
            }

            Version = version;
        }

        public string InstallationName { get; }

        public int Version { get; }

        public IReadOnlyList<string> GetUpCommands(InstallerOptions installerOptions)
        {
            var dbChangeList = new List<IDbChange>();

            AddDbChanges(dbChangeList);

            var commands = new List<string>();

            foreach (var dbChange in dbChangeList)
            {
                commands.AddRange(dbChange.GetUpScript(installerOptions));
            }

            return commands;
        }

        public IReadOnlyList<string> GetDownCommands(InstallerOptions installerOptions)
        {
            throw new NotImplementedException();
        }

        public abstract void AddDbChanges(List<IDbChange> dbChangeList);
    }
}
