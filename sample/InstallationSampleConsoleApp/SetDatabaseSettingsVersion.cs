using Microsoft.Extensions.Configuration;
using Rinsen.DatabaseInstaller;
using System.Collections.Generic;

namespace InstallationSampleConsoleApp
{
    public class SetDatabaseSettingsVersion : DatabaseVersion
    {
        private readonly IConfiguration _configuration;

        public SetDatabaseSettingsVersion(IConfiguration configuration)
            : base(1)
        {
            _configuration = configuration;
        }

        public override void AddDbChanges(List<IDbChange> dbChangeList)
        {
            var databaseSettings = dbChangeList.AddNewDatabaseSettings(); ;
            databaseSettings.CreateLogin("MyLogin2", _configuration["LoginPassword"])
                .WithUser("MyUser2")
                .AddRoleMembershipDataReader()
                .AddRoleMembershipDataWriter();
        }
    }
}
