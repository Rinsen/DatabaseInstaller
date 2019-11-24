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
            Assert.Equal("ALTER TABLE MyTable ADD\r\nMyNewColumn nvarchar(100) NULL\r\n", script);
            
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
            Assert.Equal("ALTER TABLE MyTable ADD\r\nMyNewColumn nvarchar(100) NULL,\r\nMyOtherNewColumn int NULL\r\n", script);

        }

        [Fact]
        public void AddUniqueNamedColumnsToTable_GetCorrectUpScript()
        {
            // Arrange

            // Act
            var tableAlteration = new TableAlteration("MyTable");
            tableAlteration.AddNVarCharColumn("MyNewColumn", 100).Unique("UX_MyTable_MyNewColumn");
            var script = tableAlteration.GetUpScript();

            // Assert
            Assert.Equal("ALTER TABLE MyTable ADD\r\nMyNewColumn nvarchar(100) NULL\r\n", script.Single());

        }
    }
}
