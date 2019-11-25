using Rinsen.DatabaseInstaller.SqlTypes;
using System.Linq;
using Xunit;

namespace Rinsen.DatabaseInstaller.Tests.Sql
{
    public class TableAlterationTests
    {

        [Fact]
        public void AddOneColumnToTable_GetCorrectUpScript()
        {
            // Arrange

            // Act
            var tableAlteration = new TableAlteration("MyTable");
            tableAlteration.AddNVarCharColumn("MyNewColumn", 100);
            var script = tableAlteration.GetUpScript().Single();

            // Assert
            Assert.Equal("ALTER TABLE MyTable ADD\r\nMyNewColumn nvarchar(100) NOT NULL\r\n", script);
            
        }

        [Fact]
        public void AddTwoColumnsToTable_GetCorrectUpScript()
        {
            // Arrange

            // Act
            var tableAlteration = new TableAlteration("MyTable");
            tableAlteration.AddNVarCharColumn("MyNewColumn", 100);
            tableAlteration.AddIntColumn("MyOtherNewColumn");
            var script = tableAlteration.GetUpScript().Single();

            // Assert
            Assert.Equal("ALTER TABLE MyTable ADD\r\nMyNewColumn nvarchar(100) NOT NULL,\r\nMyOtherNewColumn int NOT NULL\r\n", script);

        }

        [Fact]
        public void AddUniqueNamedColumnToTable_GetCorrectUpScript()
        {
            // Arrange

            // Act
            var tableAlteration = new TableAlteration("MyTable");
            tableAlteration.AddNVarCharColumn("MyNewColumn", 100).Unique("UX_MyTable_MyNewColumn");
            var script = tableAlteration.GetUpScript();

            // Assert
            Assert.Equal("ALTER TABLE MyTable ADD\r\nMyNewColumn nvarchar(100) NOT NULL\r\nCONSTRAINT UX_MyTable_MyNewColumn UNIQUE (MyNewColumn)\r\n", script.Single());
        }

        [Fact]
        public void AddNamedPrimaryKeyColumnToTable_GetCorrectUpScript()
        {
            // Arrange

            // Act
            var tableAlteration = new TableAlteration("MyTable");
            tableAlteration.AddNVarCharColumn("MyNewColumn", 100).PrimaryKey("PK_MyTable_MyNewColumn");
            var script = tableAlteration.GetUpScript();

            // Assert
            Assert.Equal("ALTER TABLE MyTable ADD\r\nMyNewColumn nvarchar(100) NOT NULL\r\nCONSTRAINT PK_MyTable_MyNewColumn PRIMARY KEY (MyNewColumn)\r\n", script.Single());
        }

        [Fact]
        public void AddNamedPrimaryKeyColumnAndUniqueColumnToTable_GetCorrectUpScript()
        {
            // Arrange

            // Act
            var tableAlteration = new TableAlteration("MyTable");
            tableAlteration.AddNVarCharColumn("MyNewKeyColumn", 100).PrimaryKey("PK_MyTable_MyNewKeyColumn");
            tableAlteration.AddNVarCharColumn("MyNewUniqueColumn", 100).Unique("UX_MyTable_MyNewUniqueColumn");
            var script = tableAlteration.GetUpScript();

            // Assert
            Assert.Equal("ALTER TABLE MyTable ADD\r\nMyNewKeyColumn nvarchar(100) NOT NULL,\r\nMyNewUniqueColumn nvarchar(100) NOT NULL\r\nCONSTRAINT UX_MyTable_MyNewUniqueColumn UNIQUE (MyNewUniqueColumn),\r\nCONSTRAINT PK_MyTable_MyNewKeyColumn PRIMARY KEY (MyNewKeyColumn)\r\n", script.Single());
        }
    }
}
