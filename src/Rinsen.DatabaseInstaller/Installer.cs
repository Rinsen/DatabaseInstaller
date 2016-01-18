using System;
using System.Collections.Generic;
using System.Linq;

namespace Rinsen.DatabaseInstaller
{
    public class Installer
    {
        private readonly DatabaseVersionInstaller _databaseVersionInstaller;
        private readonly VersionHandler _versionHandler;
        private readonly InstallerOptions _installerOptions;
        //private readonly ILogger _log;
        
        public Installer(DatabaseVersionInstaller changeInstaller, VersionHandler versionHandler, InstallerOptions installerOptions)//, ILoggerFactory loggerFactory)
        {
            _databaseVersionInstaller = changeInstaller;
            _versionHandler = versionHandler;
            _installerOptions = installerOptions;
          //  _log = loggerFactory.CreateLogger<Installer>();
        }
        
        public void Run(IEnumerable<DatabaseVersion> databaseVersions)
        {
            //_log.LogCritical("DatabaseInstaller started");
            if (!_versionHandler.IsInstalled())
            {
                _databaseVersionInstaller.InstallBaseVersion(new InstallerBaseVersion(_installerOptions.InstalledVersionsDatabaseTableName));
            }

            _databaseVersionInstaller.Install(databaseVersions.ToList());
            //_log.LogCritical("DatabaseInstaller finished");
        }

        public void RollbackVersions(string name, int toVersion)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<InstallationNameAndVersion> GetVersionInformation()
        {
            return _versionHandler.GetInstalledVersionsInformation();
        } 
    }
}