﻿using System.Linq;
using Rinsen.DatabaseInstaller.Generic.Sql;
using Xunit;

namespace Rinsen.DatabaseInstaller.Tests.Sql
{
    public class GenericTableAlterationTests
    {

        class MyData
        {
            public string MyNewColumn { get; set; }

            public int MyOtherNewColumn { get; set; }
        }

        [Fact]
        public void AddOneColumnToTable_GetCorrectUpScript()
        {
            // Arrange

            // Act
            var tableAlteration = new TableAlteration<MyData>("MyTable");
            tableAlteration.AddColumn(m => m.MyNewColumn, 100);
            var script = tableAlteration.GetUpScript().Single();

            // Assert
            Assert.Equal("ALTER TABLE MyTable ADD\r\nMyNewColumn nvarchar(100)\r\n", script);
            
        }

        [Fact]
        public void AddTwoColumnsToTable_GetCorrectUpScript()
        {
            // Arrange

            // Act
            var tableAlteration = new TableAlteration<MyData>("MyTable");
            tableAlteration.AddColumn(m => m.MyNewColumn, 100);
            tableAlteration.AddColumn(m => m.MyOtherNewColumn);
            var script = tableAlteration.GetUpScript().Single();

            // Assert
            Assert.Equal("ALTER TABLE MyTable ADD\r\nMyNewColumn nvarchar(100),\r\nMyOtherNewColumn int\r\n", script);

        }
    }
}
