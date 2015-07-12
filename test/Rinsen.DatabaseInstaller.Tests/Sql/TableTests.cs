using Rinsen.DatabaseInstaller.Sql;
using Xunit;

namespace Rinsen.DatabaseInstaller.Tests.Sql
{
    public class TableTests
    {
        [Fact]
        public void WhenCreateTable_GetCorrespondingTableScript()
        {
            var table = new Table("TestTable");
            table.AddColumn("MyColumn", new Int());

            var createScript = table.GetUpScript();

            Assert.Equal("CREATE TABLE TestTable\r\n(\r\nMyColumn int\r\n)", createScript);
        }

        [Fact]
        public void WhenCreateTableWithAutoIncrementAndPrimaryKey_GetCorrespondingTableScript()
        {
            var table = new Table("TestTable");
            table.AddColumn("MyIdColumn", new Int()).AutoIncrement().PrimaryKey();
            table.AddColumn("MyValue", new NVarChar(100));

            var createScript = table.GetUpScript();

            Assert.Equal("CREATE TABLE TestTable\r\n(\r\nMyIdColumn int IDENTITY(1,1) PRIMARY KEY,\r\nMyValue nvarchar(100)\r\n)", createScript);
        }

        [Fact]
        public void WhenCreateTableWithNotNullAndPrimaryKey_GetCorrespondingTableScript()
        {
            var table = new Table("TestTable");
            table.AddColumn("MyIdColumn", new Int()).PrimaryKey();
            table.AddColumn("MyValue", new NVarChar(100));

            var createScript = table.GetUpScript();

            Assert.Equal("CREATE TABLE TestTable\r\n(\r\nMyIdColumn int NOT NULL PRIMARY KEY,\r\nMyValue nvarchar(100)\r\n)", createScript);
        }

        [Fact]
        public void WhenCreateTableWithNotNullAndTwoColumnsAsPrimaryKey_GetCorrespondingTableScript()
        {
            var table = new Table("TestTable");
            table.AddColumn("MyIdColumn1", new Int()).PrimaryKey();
            table.AddColumn("MyIdColumn2", new Int()).PrimaryKey();
            table.AddColumn("MyValue", new NVarChar(100));

            var createScript = table.GetUpScript();

            Assert.Equal("CREATE TABLE TestTable\r\n(\r\nMyIdColumn1 int NOT NULL,\r\nMyIdColumn2 int NOT NULL,\r\nMyValue nvarchar(100),\r\nCONSTRAINT PK_TestTable PRIMARY KEY (MyIdColumn2,MyIdColumn1)\r\n)", createScript);
        }

        [Fact]
        public void WhenCreateTableWithNamedPrimaryKey_GetCorrespondingTableScript()
        {
            var table = new Table("TestTable");
            table.AddColumn("Key1", new Int()).PrimaryKey("PrimaryKeyForTestTable");
            table.AddColumn("Key2", new Int()).PrimaryKey("PrimaryKeyForTestTable");

            var createScript = table.GetUpScript();

            Assert.Equal("CREATE TABLE TestTable\r\n(\r\nKey1 int NOT NULL,\r\nKey2 int NOT NULL,\r\nCONSTRAINT PrimaryKeyForTestTable PRIMARY KEY (Key1,Key2)\r\n)", createScript);
        }

        [Fact]
        public void WhenCreateTableWithNamedUniquesAndOneUniqueColumn_GetCorrespondingTableScript()
        {
            var table = new Table("TestTable");
            table.AddColumn("Col1", new Int()).Unique("PrimaryKeyForTestTable");
            table.AddColumn("Col2", new Int()).Unique("PrimaryKeyForTestTable");
            table.AddColumn("Col3", new Int()).Unique();

            var createScript = table.GetUpScript();

            Assert.Equal("CREATE TABLE TestTable\r\n(\r\nCol1 int,\r\nCol2 int,\r\nCol3 int UNIQUE,\r\nCONSTRAINT PrimaryKeyForTestTable UNIQUE (Col1,Col2)\r\n)", createScript);
        }

        [Fact]
        public void WhenColumnWithNotNull_GetCorrespondingTableScript()
        {
            var table = new Table("TestTable");
            table.AddColumn("MyColumn", new Int()).NotNull();

            var createScript = table.GetUpScript();

            Assert.Equal("CREATE TABLE TestTable\r\n(\r\nMyColumn int NOT NULL\r\n)", createScript);
        }



    }
}
