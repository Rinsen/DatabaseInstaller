﻿using System.Linq;
using Xunit;

namespace Rinsen.DatabaseInstaller.Tests.Generic.Sql
{
    public class GenericIndexTests
    {
        class MyTable
        {
            public int MyColumn { get; set; }
        }

        [Fact]
        public void CreateIndexWithOneColumn_GetOneColumnAdded()
        {
            // Arrange
            var index = new Index<MyTable>("MyIndexName", "MyTable");

            // Act
            index.AddColumn(m => m.MyColumn);

            // Assert
            Assert.Equal("MyIndexName", index.Name);
            Assert.Equal("MyTable", index.TableName);
            Assert.Single(index.Columns);
            Assert.Equal("MyColumn", index.Columns.First());
        }

        [Fact]
        public void CreateIndexWithOneColumn_GetValidCreateScript()
        {
            // Arrange
            var index = new Index<MyTable>("MyIndexName", "MyTable");

            // Act
            index.AddColumn(m => m.MyColumn);

            // Assert
            Assert.Single(index.GetUpScript());
            Assert.Equal("CREATE INDEX MyIndexName \r\nON MyTable(MyColumn)\r\n", index.GetUpScript().First());
        }
    }
}
