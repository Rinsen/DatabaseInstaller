using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Rinsen.DatabaseInstaller.Tests.Sql
{
    public class DatabaseTests
    {
        [Fact]
        public void WhenCreateDatabase_GetCorrespondingScript()
        {
            var database = new Database("TestDatabase");
            database.CreateDatabase();

            var createScript = database.GetUpScript().Single();

            Assert.Equal("CREATE DATABASE TestDatabase", createScript);
        }

        [Fact]
        public void WhenCreateDatabaseAndUser_GetCorrespondingTableScript()
        {
            var database = new Database("TestDatabase");
            database.CreateDatabase();
            database.CreateLogin("LoginName");
            database.CreateUser("UserName");
            database.AddRoleMembershipDataReader("UserName");
            database.AddRoleMembershipDataWriter("UserName");

            var createScript = database.GetUpScript();

            Assert.Equal("CREATE DATABASE TestDatabase", createScript.First());
        }

    }
}
