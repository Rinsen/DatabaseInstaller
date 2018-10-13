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

            Assert.Equal("CREATE TABLE [TestTables]\r\n(\r\n[MyColumn] int NULL\r\n)", createScript);
        }

        [Fact]
        public void WhenCreateTableWithAutoIncrementAndPrimaryKey_GetCorrespondingTableScript()
        {
            var table = new Table("TestTables");
            table.AddColumn("MyIdColumn", new Int()).AutoIncrement().PrimaryKey();
            table.AddColumn("MyValue", new NVarChar(100));

            var createScript = table.GetUpScript().Single();

            Assert.Equal("CREATE TABLE [TestTables]\r\n(\r\n[MyIdColumn] int IDENTITY(1,1) PRIMARY KEY,\r\n[MyValue] nvarchar(100) NULL\r\n)", createScript);
        }

        [Fact]
        public void WhenCreateTableWithNotNullAndPrimaryKey_GetCorrespondingTableScript()
        {
            var table = new Table("TestTables");
            table.AddColumn("MyIdColumn", new Int()).PrimaryKey();
            table.AddColumn("MyValue", new NVarChar(100));

            var createScript = table.GetUpScript().Single();

            Assert.Equal("CREATE TABLE [TestTables]\r\n(\r\n[MyIdColumn] int NOT NULL PRIMARY KEY,\r\n[MyValue] nvarchar(100) NULL\r\n)", createScript);
        }

        [Fact]
        public void WhenCreateTableWithNotNullAndTwoColumnsAsPrimaryKey_GetCorrespondingTableScript()
        {
            var table = new Table("TestTables");
            table.AddColumn("MyIdColumn1", new Int()).PrimaryKey();
            table.AddColumn("MyIdColumn2", new Int()).PrimaryKey();
            table.AddColumn("MyValue", new NVarChar(100));

            var createScript = table.GetUpScript().Single();

            Assert.Equal("CREATE TABLE [TestTables]\r\n(\r\n[MyIdColumn1] int NOT NULL,\r\n[MyIdColumn2] int NOT NULL,\r\n[MyValue] nvarchar(100) NULL,\r\nCONSTRAINT PK_TestTables PRIMARY KEY (MyIdColumn2,MyIdColumn1)\r\n)", createScript);
        }

        [Fact]
        public void WhenCreateTableWithNamedPrimaryKey_GetCorrespondingTableScript()
        {
            var table = new Table("TestTables");
            table.AddColumn("Key1", new Int()).PrimaryKey("PrimaryKeyForTestTables");
            table.AddColumn("Key2", new Int()).PrimaryKey("PrimaryKeyForTestTables");

            var createScript = table.GetUpScript().Single();

            Assert.Equal("CREATE TABLE [TestTables]\r\n(\r\n[Key1] int NOT NULL,\r\n[Key2] int NOT NULL,\r\nCONSTRAINT PrimaryKeyForTestTables PRIMARY KEY (Key1,Key2)\r\n)", createScript);
        }

        [Fact]
        public void WhenCreateTableWithNamedUniquesAndOneUniqueColumn_GetCorrespondingTableScript()
        {
            var table = new Table("TestTables");
            table.AddColumn("Col1", new Int()).Unique("PrimaryKeyForTestTables");
            table.AddColumn("Col2", new Int()).Unique("PrimaryKeyForTestTables");
            table.AddColumn("Col3", new Int()).Unique();

            var createScript = table.GetUpScript().Single();

            Assert.Equal("CREATE TABLE [TestTables]\r\n(\r\n[Col1] int NULL,\r\n[Col2] int NULL,\r\n[Col3] int NULL UNIQUE,\r\nCONSTRAINT PrimaryKeyForTestTables UNIQUE (Col1,Col2)\r\n)", createScript);
        }

        [Fact]
        public void WhenColumnWithNotNull_GetCorrespondingTableScript()
        {
            var table = new Table("TestTables");
            table.AddColumn("MyColumn", new Int()).NotNull();

            var createScript = table.GetUpScript().Single();

            Assert.Equal("CREATE TABLE [TestTables]\r\n(\r\n[MyColumn] int NOT NULL\r\n)", createScript);
        }

        [Fact]
        public void WhenGuidColumn_GetCorrespondingTableScript()
        {
            var table = new Table("TestTables");
            table.AddColumn("MyColumn", new Guid()).NotNull();

            var createScript = table.GetUpScript().Single();

            Assert.Equal("CREATE TABLE [TestTables]\r\n(\r\n[MyColumn] uniqueidentifier NOT NULL\r\n)", createScript);
        }

        [Fact]
        public void WhenCreateTableWithAutoIncrementAsNoPrimaryKey_GetCorrespondingTableScript()
        {
            var table = new Table("TestTables");
            table.AddColumn("MyIdColumn", new Int()).AutoIncrement(primaryKey: false);
            table.AddColumn("MyValue", new NVarChar(100));

            var createScript = table.GetUpScript().Single();

            Assert.Equal("CREATE TABLE [TestTables]\r\n(\r\n[MyIdColumn] int IDENTITY(1,1),\r\n[MyValue] nvarchar(100) NULL\r\n)", createScript);
        }


    }
}
