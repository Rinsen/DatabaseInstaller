﻿using Rinsen.DatabaseInstaller.SqlTypes;
using System.Linq;
using Xunit;

namespace Rinsen.DatabaseInstaller.Tests.Sql
{
    public class UniqueClusteredIndexTests
    {
        [Fact]
        public void GetCreateScript_CreateScriptIsCorrect()
        {
            // Arrange
            var index = new UniqueClusteredIndex("MyIndex", "MyTable");
            index.AddColumn("MyColumn");

            // Act
            var createScripts = index.GetUpScript();

            // Assert
            Assert.Single(createScripts);
            Assert.Equal("CREATE UNIQUE CLUSTERED INDEX MyIndex \r\nON MyTable(MyColumn)\r\n", createScripts.First());
        }
    }
}
