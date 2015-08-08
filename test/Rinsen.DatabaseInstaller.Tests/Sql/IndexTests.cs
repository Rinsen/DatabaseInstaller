using Rinsen.DatabaseInstaller.Sql;
using System;
using System.Linq;
using Xunit;

namespace Rinsen.DatabaseInstaller.Tests.Sql
{
    public class IndexTests
    {
        [Fact]
        public void CreateIndexWithOneColumn_GetOneColumnAdded()
        {
            // Arrange
            var index = new Index("MyIndexName", "MyTable");

            // Act
            index.AddColumn("MyColumn");

            // Assert
            Assert.Equal("MyIndexName", index.Name);
            Assert.Equal("MyTable", index.TableName);
            Assert.Equal(1, index.Columns.Count);
            Assert.Equal("MyColumn", index.Columns.First());
        }

        [Fact]
        public void CreateIndexWithOneColumn_GetValidCreateScript()
        {
            // Arrange
            var index = new Index("MyIndexName", "MyTable");

            // Act
            index.AddColumn("MyColumn");

            // Assert
            Assert.Equal(1, index.GetUpScript().Count);
            Assert.Equal("CREATE INDEX MyIndexName \r\nON MyTable(MyColumn)\r\n", index.GetUpScript().First());
        }

        [Fact]
        public void CreateIndexWithTwoColumns_GetValidCreateScript()
        {
            // Arrange
            var index = new Index("MyIndexName", "MyTable");

            // Act
            index.AddColumn("MyColumn");
            index.AddColumn("MyOtherColumn");

            // Assert
            Assert.Equal(1, index.GetUpScript().Count);
            Assert.Equal(2, index.Columns.Count);
            Assert.Equal("CREATE INDEX MyIndexName \r\nON MyTable(MyColumn, MyOtherColumn)\r\n", index.GetUpScript().First());
        }

        [Fact]
        public void CreateIndexWithSameColumnTwoTimes_GetCorrectException()
        {
            // Arrange
            var index = new Index("MyIndexName", "MyTable");

            // Act
            index.AddColumn("MyColumn");

            // Assert
            var ex = Assert.Throws<ArgumentException>(() => index.AddColumn("MyColumn"));
            Assert.Equal("The column MyColumn is already added to index MyIndexName on table MyTable", ex.Message);
            
        }
    }
}

