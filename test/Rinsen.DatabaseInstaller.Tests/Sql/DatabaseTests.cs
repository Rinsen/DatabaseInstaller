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
        public void WhenCreateDatabaseAndUser_GetCorrespondingTableScript()
        {
            var database = new DatabaseSettings();
            database.CreateLogin("LoginName").WithUser().AddRoleMembershipDataReader().AddRoleMembershipDataWriter();
            database.CreateUser("UserName").ForLogin("UsersLoginName");

            var createScript = database.GetUpScript(TestHelper.GetInstallerOptions());

            Assert.Equal("USE [TestDb]", createScript[0]);
            Assert.StartsWith($"IF 'LoginName' NOT IN (SELECT [name] FROM [master].[sys].[sql_logins]){Environment.NewLine}CREATE LOGIN LoginName WITH PASSWORD = '", createScript[1]);
            Assert.Equal($"IF 'LoginName' NOT IN (SELECT [name] FROM [TestDb].[sys].[sysusers]){Environment.NewLine}CREATE USER LoginName FOR LOGIN LoginName", createScript[2]);
            Assert.Equal("ALTER ROLE db_datareader ADD MEMBER LoginName", createScript[3]);
            Assert.Equal("ALTER ROLE db_datawriter ADD MEMBER LoginName", createScript[4]);
            Assert.Equal($"IF 'UserName' NOT IN (SELECT [name] FROM [TestDb].[sys].[sysusers]){Environment.NewLine}CREATE USER UserName FOR LOGIN UsersLoginName", createScript[5]);
        }

    }
}
