using System.Linq;
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
            Assert.Equal("MyIndexName", index.IndexName);
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
            var upScripts = index.GetUpScript(TestHelper.GetInstallerOptions());

            // Assert
            Assert.Single(upScripts);
            Assert.Equal($"CREATE INDEX MyIndexName {System.Environment.NewLine}ON [TestDb].[dbo].[MyTable] (MyColumn){System.Environment.NewLine}", upScripts.First());
        }

        [Fact]
        public void GetUniqueClusteredCreateScript_CreateScriptIsCorrect()
        {
            // Arrange
            var index = new Index<MyTable>("MyIndex", "MyTable").Unique().Clustered();
            index.AddColumn(m => m.MyColumn);

            // Act
            var createScripts = index.GetUpScript(TestHelper.GetInstallerOptions());

            // Assert
            Assert.Single(createScripts);
            Assert.Equal($"CREATE UNIQUE CLUSTERED INDEX MyIndex {System.Environment.NewLine}ON [TestDb].[dbo].[MyTable] (MyColumn){System.Environment.NewLine}", createScripts.First());
        }

        [Fact]
        public void GetClusteredCreateScript_CreateScriptIsCorrect()
        {
            // Arrange
            var index = new Index<MyTable>("MyIndex", "MyTable").Clustered();
            index.AddColumn(m => m.MyColumn);

            // Act
            var createScripts = index.GetUpScript(TestHelper.GetInstallerOptions());  

            // Assert
            Assert.Single(createScripts);
            Assert.Equal($"CREATE CLUSTERED INDEX MyIndex {System.Environment.NewLine}ON [TestDb].[dbo].[MyTable] (MyColumn){System.Environment.NewLine}", createScripts.First());
        }

        [Fact]
        public void GetUniqueCreateScript_CreateScriptIsCorrect()
        {
            // Arrange
            var index = new Index<MyTable>("MyIndex", "MyTable").Unique();
            index.AddColumn(m => m.MyColumn);

            // Act
            var createScripts = index.GetUpScript(TestHelper.GetInstallerOptions());

            // Assert
            Assert.Single(createScripts);
            Assert.Equal($"CREATE UNIQUE INDEX MyIndex {System.Environment.NewLine}ON [TestDb].[dbo].[MyTable] (MyColumn){System.Environment.NewLine}", createScripts.First());
        }
    }
}
