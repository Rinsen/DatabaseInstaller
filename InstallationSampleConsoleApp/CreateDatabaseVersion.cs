using Rinsen.DatabaseInstaller;
using System;
using System.Collections.Generic;
using System.Text;

namespace InstallationSampleConsoleApp
{
    public class CreateDatabaseVersion : DatabaseVersion
    {
        public CreateDatabaseVersion()
            : base(1)
        {
        }

        public override void AddDbChanges(List<IDbChange> dbChangeList)
        {
            var database = new Database("TestDb1234");
            database.CreateLogin("MyLogin");
            database.CreateUser("MyUser");
            

            dbChangeList.Add(database);

        }
    }
}
