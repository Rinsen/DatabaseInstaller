using Rinsen.DatabaseInstaller.SqlTypes;
using System;
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
            var script = tableAlteration.GetUpScript(TestHelper.GetInstallerOptions()).Single();

            // Assert
            Assert.Equal($"ALTER TABLE [TestDb].[dbo].[MyTable] ADD{Environment.NewLine}MyNewColumn nvarchar(100) NOT NULL{Environment.NewLine}", script);
            
        }

        [Fact]
        public void AddTwoColumnsToTable_GetCorrectUpScript()
        {
            // Arrange

            // Act
            var tableAlteration = new TableAlteration("MyTable");
            tableAlteration.AddNVarCharColumn("MyNewColumn", 100);
            tableAlteration.AddIntColumn("MyOtherNewColumn");
            var script = tableAlteration.GetUpScript(TestHelper.GetInstallerOptions()).Single();

            // Assert
            Assert.Equal($"ALTER TABLE [TestDb].[dbo].[MyTable] ADD{Environment.NewLine}MyNewColumn nvarchar(100) NOT NULL,{Environment.NewLine}MyOtherNewColumn int NOT NULL{Environment.NewLine}", script);

        }

        [Fact]
        public void AddUniqueNamedColumnToTable_GetCorrectUpScript()
        {
            // Arrange

            // Act
            var tableAlteration = new TableAlteration("MyTable");
            tableAlteration.AddNVarCharColumn("MyNewColumn", 100).Unique("UX_MyTable_MyNewColumn");
            var script = tableAlteration.GetUpScript(TestHelper.GetInstallerOptions());

            // Assert
            Assert.Equal($"ALTER TABLE [TestDb].[dbo].[MyTable] ADD{Environment.NewLine}MyNewColumn nvarchar(100) NOT NULL{Environment.NewLine}CONSTRAINT UX_MyTable_MyNewColumn UNIQUE (MyNewColumn){Environment.NewLine}", script.Single());
        }

        [Fact]
        public void AddNamedPrimaryKeyColumnToTable_GetCorrectUpScript()
        {
            // Arrange

            // Act
            var tableAlteration = new TableAlteration("MyTable");
            tableAlteration.AddNVarCharColumn("MyNewColumn", 100).PrimaryKey("PK_MyTable_MyNewColumn");
            var script = tableAlteration.GetUpScript(TestHelper.GetInstallerOptions());

            // Assert
            Assert.Equal($"ALTER TABLE [TestDb].[dbo].[MyTable] ADD{Environment.NewLine}MyNewColumn nvarchar(100) NOT NULL{Environment.NewLine}CONSTRAINT PK_MyTable_MyNewColumn PRIMARY KEY (MyNewColumn){Environment.NewLine}", script.Single());
        }

        [Fact]
        public void AddNamedPrimaryKeyColumnAndUniqueColumnToTable_GetCorrectUpScript()
        {
            // Arrange

            // Act
            var tableAlteration = new TableAlteration("MyTable");
            tableAlteration.AddNVarCharColumn("MyNewKeyColumn", 100).PrimaryKey("PK_MyTable_MyNewKeyColumn");
            tableAlteration.AddNVarCharColumn("MyNewUniqueColumn", 100).Unique("UX_MyTable_MyNewUniqueColumn");
            var script = tableAlteration.GetUpScript(TestHelper.GetInstallerOptions());

            // Assert
            Assert.Equal($"ALTER TABLE [TestDb].[dbo].[MyTable] ADD{Environment.NewLine}MyNewKeyColumn nvarchar(100) NOT NULL,{Environment.NewLine}MyNewUniqueColumn nvarchar(100) NOT NULL{Environment.NewLine}CONSTRAINT UX_MyTable_MyNewUniqueColumn UNIQUE (MyNewUniqueColumn),{Environment.NewLine}CONSTRAINT PK_MyTable_MyNewKeyColumn PRIMARY KEY (MyNewKeyColumn){Environment.NewLine}", script.Single());
        }
    }
}
