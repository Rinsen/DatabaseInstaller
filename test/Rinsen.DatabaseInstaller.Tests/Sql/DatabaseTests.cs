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

            var createScript = database.GetUpScript(TestHelper.GetInstallerOptions()).First();

            Assert.Equal("IF 'TestDatabase' NOT IN (SELECT [name] FROM [master].[sys].[databases] WHERE [name] NOT IN ('master', 'tempdb', 'model', 'msdb'))\r\nCREATE DATABASE TestDatabase", createScript);
        }

        [Fact]
        public void WhenCreateDatabaseAndUser_GetCorrespondingTableScript()
        {
            var database = new Database("TestDatabase");
            database.CreateLogin("LoginName").WithUser().AddRoleMembershipDataReader().AddRoleMembershipDataWriter();
            database.CreateUser("UserName").ForLogin("UsersLoginName");

            var createScript = database.GetUpScript(TestHelper.GetInstallerOptions());

            Assert.Equal("IF 'TestDatabase' NOT IN (SELECT [name] FROM [master].[sys].[databases] WHERE [name] NOT IN ('master', 'tempdb', 'model', 'msdb'))\r\nCREATE DATABASE TestDatabase", createScript[0]);
            Assert.Equal("USE TestDatabase", createScript[1]);
            Assert.StartsWith("IF 'LoginName' NOT IN (SELECT [name] FROM [master].[sys].[sql_logins])\r\nCREATE LOGIN Kalle WITH PASSWORD = '", createScript[2]);
            Assert.Equal("IF 'LoginName' NOT IN (SELECT [name] FROM [TestDatabase].[sys].[sysusers])\r\nCRATE USER LoginName FOR LOGIN LoginName", createScript[3]);
            Assert.Equal("ALTER ROLE db_datareader ADD MEMBER LoginName", createScript[4]);
            Assert.Equal("ALTER ROLE db_datawriter ADD MEMBER LoginName", createScript[5]);
            Assert.Equal("IF 'UserName' NOT IN (SELECT [name] FROM [TestDatabase].[sys].[sysusers])\r\nCRATE USER UserName FOR LOGIN UsersLoginName", createScript[6]);
        }

    }
}
