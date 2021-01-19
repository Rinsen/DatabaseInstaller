using Rinsen.DatabaseInstaller;
using System.Collections.Generic;

namespace InstallationSampleConsoleApp
{
    public class SetDatabaseSettingsVersion : DatabaseVersion
    {
        public SetDatabaseSettingsVersion()
            : base(1)
        {
        }

        public override void AddDbChanges(List<IDbChange> dbChangeList)
        {
            
            var databaseSettings = dbChangeList.AddNewDatabaseSettings(); ;
            databaseSettings.CreateLogin("MyLogin2")
                .WithUser("MyUser2")
                .AddRoleMembershipDataReader()
                .AddRoleMembershipDataWriter();
        }
    }
}
