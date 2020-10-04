using Rinsen.DatabaseInstaller;
using System.Collections.Generic;

namespace InstallationSampleConsoleApp
{
    public class CreateDatabaseVersion : DatabaseVersion
    {
        public CreateDatabaseVersion()
            : base(0)
        {
        }

        public override void AddDbChanges(List<IDbChange> dbChangeList)
        {
            var database = new Database("TestDb1234");
            database.CreateLogin("MyLogin")
                .WithUser("MyUser")
                .AddRoleMembershipDataReader()
                .AddRoleMembershipDataWriter();

            dbChangeList.Add(database);
        }
    }
}
