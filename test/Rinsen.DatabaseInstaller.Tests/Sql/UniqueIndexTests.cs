﻿using Rinsen.DatabaseInstaller.Sql;
using System.Linq;
using Xunit;

namespace Rinsen.DatabaseInstaller.Tests.Sql
{
    public class UniqueIndexTests
    {
        [Fact]
        public void GetCreateScript_CreateScriptIsCorrect()
        {
            // Arrange
            var index = new UniqueIndex("MyIndex", "MyTable");
            index.AddColumn("MyColumn");

            // Act
            var createScripts = index.GetUpScript();

            // Assert
            Assert.Equal(1, createScripts.Count);
            Assert.Equal("CREATE UNIQUE INDEX MyIndex \r\nON MyTable(MyColumn)\r\n", createScripts.First());
        }

    }
}