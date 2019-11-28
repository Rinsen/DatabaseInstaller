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

            // Assert
            Assert.Single(index.GetUpScript());
            Assert.Equal("CREATE INDEX MyIndexName \r\nON MyTable(MyColumn)\r\n", index.GetUpScript().First());
        }

        [Fact]
        public void GetUniqueClusteredCreateScript_CreateScriptIsCorrect()
        {
            // Arrange
            var index = new Index<MyTable>("MyIndex", "MyTable").Unique().Clustered();
            index.AddColumn(m => m.MyColumn);

            // Act
            var createScripts = index.GetUpScript();

            // Assert
            Assert.Single(createScripts);
            Assert.Equal("CREATE UNIQUE CLUSTERED INDEX MyIndex \r\nON MyTable(MyColumn)\r\n", createScripts.First());
        }

        [Fact]
        public void GetClusteredCreateScript_CreateScriptIsCorrect()
        {
            // Arrange
            var index = new Index<MyTable>("MyIndex", "MyTable").Clustered();
            index.AddColumn(m => m.MyColumn);

            // Act
            var createScripts = index.GetUpScript();  

            // Assert
            Assert.Single(createScripts);
            Assert.Equal("CREATE CLUSTERED INDEX MyIndex \r\nON MyTable(MyColumn)\r\n", createScripts.First());
        }

        [Fact]
        public void GetUniqueCreateScript_CreateScriptIsCorrect()
        {
            // Arrange
            var index = new Index<MyTable>("MyIndex", "MyTable").Unique();
            index.AddColumn(m => m.MyColumn);

            // Act
            var createScripts = index.GetUpScript();

            // Assert
            Assert.Single(createScripts);
            Assert.Equal("CREATE UNIQUE INDEX MyIndex \r\nON MyTable(MyColumn)\r\n", createScripts.First());
        }
    }
}
