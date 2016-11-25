using Rinsen.DatabaseInstaller.SqlTypes;
using System.Linq;
using Xunit;

namespace Rinsen.DatabaseInstaller.Tests.Sql
{
    public class TableTests
    {
        [Fact]
        public void WhenCreateTable_GetCorrespondingTableScript()
        {
            var table = new Table("TestTables");
            table.AddColumn("MyColumn", new Int());

            var createScript = table.GetUpScript().Single();

            Assert.Equal("CREATE TABLE TestTables\r\n(\r\nMyColumn int\r\n)", createScript);
        }

        [Fact]
        public void WhenCreateTableWithAutoIncrementAndPrimaryKey_GetCorrespondingTableScript()
        {
            var table = new Table("TestTables");
            table.AddColumn("MyIdColumn", new Int()).AutoIncrement().PrimaryKey();
            table.AddColumn("MyValue", new NVarChar(100));

            var createScript = table.GetUpScript().Single();

            Assert.Equal("CREATE TABLE TestTables\r\n(\r\nMyIdColumn int IDENTITY(1,1) PRIMARY KEY,\r\nMyValue nvarchar(100)\r\n)", createScript);
        }

        [Fact]
        public void WhenCreateTableWithNotNullAndPrimaryKey_GetCorrespondingTableScript()
        {
            var table = new Table("TestTables");
            table.AddColumn("MyIdColumn", new Int()).PrimaryKey();
            table.AddColumn("MyValue", new NVarChar(100));

            var createScript = table.GetUpScript().Single();

            Assert.Equal("CREATE TABLE TestTables\r\n(\r\nMyIdColumn int NOT NULL PRIMARY KEY,\r\nMyValue nvarchar(100)\r\n)", createScript);
        }

        [Fact]
        public void WhenCreateTableWithNotNullAndTwoColumnsAsPrimaryKey_GetCorrespondingTableScript()
        {
            var table = new Table("TestTables");
            table.AddColumn("MyIdColumn1", new Int()).PrimaryKey();
            table.AddColumn("MyIdColumn2", new Int()).PrimaryKey();
            table.AddColumn("MyValue", new NVarChar(100));

            var createScript = table.GetUpScript().Single();

            Assert.Equal("CREATE TABLE TestTables\r\n(\r\nMyIdColumn1 int NOT NULL,\r\nMyIdColumn2 int NOT NULL,\r\nMyValue nvarchar(100),\r\nCONSTRAINT PK_TestTables PRIMARY KEY (MyIdColumn2,MyIdColumn1)\r\n)", createScript);
        }

        [Fact]
        public void WhenCreateTableWithNamedPrimaryKey_GetCorrespondingTableScript()
        {
            var table = new Table("TestTables");
            table.AddColumn("Key1", new Int()).PrimaryKey("PrimaryKeyForTestTables");
            table.AddColumn("Key2", new Int()).PrimaryKey("PrimaryKeyForTestTables");

            var createScript = table.GetUpScript().Single();

            Assert.Equal("CREATE TABLE TestTables\r\n(\r\nKey1 int NOT NULL,\r\nKey2 int NOT NULL,\r\nCONSTRAINT PrimaryKeyForTestTables PRIMARY KEY (Key1,Key2)\r\n)", createScript);
        }

        [Fact]
        public void WhenCreateTableWithNamedUniquesAndOneUniqueColumn_GetCorrespondingTableScript()
        {
            var table = new Table("TestTables");
            table.AddColumn("Col1", new Int()).Unique("PrimaryKeyForTestTables");
            table.AddColumn("Col2", new Int()).Unique("PrimaryKeyForTestTables");
            table.AddColumn("Col3", new Int()).Unique();

            var createScript = table.GetUpScript().Single();

            Assert.Equal("CREATE TABLE TestTables\r\n(\r\nCol1 int,\r\nCol2 int,\r\nCol3 int UNIQUE,\r\nCONSTRAINT PrimaryKeyForTestTables UNIQUE (Col1,Col2)\r\n)", createScript);
        }

        [Fact]
        public void WhenColumnWithNotNull_GetCorrespondingTableScript()
        {
            var table = new Table("TestTables");
            table.AddColumn("MyColumn", new Int()).NotNull();

            var createScript = table.GetUpScript().Single();

            Assert.Equal("CREATE TABLE TestTables\r\n(\r\nMyColumn int NOT NULL\r\n)", createScript);
        }

        [Fact]
        public void WhenGuidColumn_GetCorrespondingTableScript()
        {
            var table = new Table("TestTables");
            table.AddColumn("MyColumn", new Guid()).NotNull();

            var createScript = table.GetUpScript().Single();

            Assert.Equal("CREATE TABLE TestTables\r\n(\r\nMyColumn uniqueidentifier NOT NULL\r\n)", createScript);
        }

        [Fact]
        public void WhenCreateTableWithAutoIncrementAsNoPrimaryKey_GetCorrespondingTableScript()
        {
            var table = new Table("TestTables");
            table.AddColumn("MyIdColumn", new Int()).AutoIncrement(primaryKey: false);
            table.AddColumn("MyValue", new NVarChar(100));

            var createScript = table.GetUpScript().Single();

            Assert.Equal("CREATE TABLE TestTables\r\n(\r\nMyIdColumn int IDENTITY(1,1),\r\nMyValue nvarchar(100)\r\n)", createScript);
        }


    }
}
