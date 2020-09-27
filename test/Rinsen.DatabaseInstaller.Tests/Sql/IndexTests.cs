using Rinsen.DatabaseInstaller.SqlTypes;
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
            Assert.Equal("MyIndexName", index.IndexName);
            Assert.Equal("MyTable", index.TableName);
            Assert.Single(index.Columns);
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
            Assert.Single(index.GetUpScript(TestHelper.GetInstallerOptions()));
            Assert.Equal("CREATE INDEX MyIndexName \r\nON [TestDb].[dbo].[MyTable] (MyColumn)\r\n", index.GetUpScript(TestHelper.GetInstallerOptions()).First());
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
            Assert.Single(index.GetUpScript(TestHelper.GetInstallerOptions()));
            Assert.Equal(2, index.Columns.Count);
            Assert.Equal("CREATE INDEX MyIndexName \r\nON [TestDb].[dbo].[MyTable] (MyColumn, MyOtherColumn)\r\n", index.GetUpScript(TestHelper.GetInstallerOptions()).First());
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

        [Fact]
        public void TryToGetUpScriptOnIndexWithNoColumnsAdded_GetExceptionWithFaultMessage()
        {
            // Arrange
            var index = new Index("Name", "MyTable");

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => index.GetUpScript(TestHelper.GetInstallerOptions()));
            Assert.Equal("No columns is added to index Name for table MyTable", ex.Message);

        }

        [Fact]
        public void GetClusteredCreateScript_CreateScriptIsCorrect()
        {
            // Arrange
            var index = new Index("MyIndex", "MyTable").Clustered();
            index.AddColumn("MyColumn");

            // Act
            var createScripts = index.GetUpScript(TestHelper.GetInstallerOptions());

            // Assert
            Assert.Single(createScripts);
            Assert.Equal("CREATE CLUSTERED INDEX MyIndex \r\nON [TestDb].[dbo].[MyTable] (MyColumn)\r\n", createScripts.First());
        }

        [Fact]
        public void GetUniqueClusteredCreateScript_CreateScriptIsCorrect()
        {
            // Arrange
            var index = new Index("MyIndex", "MyTable").Unique().Clustered();
            index.AddColumn("MyColumn");

            // Act
            var createScripts = index.GetUpScript(TestHelper.GetInstallerOptions());

            // Assert
            Assert.Single(createScripts);
            Assert.Equal("CREATE UNIQUE CLUSTERED INDEX MyIndex \r\nON [TestDb].[dbo].[MyTable] (MyColumn)\r\n", createScripts.First());
        }

        [Fact]
        public void GetUniqueCreateScript_CreateScriptIsCorrect()
        {
            // Arrange
            var index = new Index("MyIndex", "MyTable").Unique();
            index.AddColumn("MyColumn");

            // Act
            var createScripts = index.GetUpScript(TestHelper.GetInstallerOptions());

            // Assert
            Assert.Single(createScripts);
            Assert.Equal("CREATE UNIQUE INDEX MyIndex \r\nON [TestDb].[dbo].[MyTable] (MyColumn)\r\n", createScripts.First());
        }
    }
}

