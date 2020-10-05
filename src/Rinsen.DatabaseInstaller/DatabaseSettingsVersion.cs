using System;
using System.Collections.Generic;

namespace Rinsen.DatabaseInstaller
{
    public abstract class DatabaseSettingsVersion
    {
        /// <summary>
        /// Database version description
        /// </summary>
        /// <param name="version">Version number</param>
        /// <param name="installationName">Installation name, if none specified the default will be this class namespace</param>
        public DatabaseSettingsVersion(int version, string installationName = null)
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
