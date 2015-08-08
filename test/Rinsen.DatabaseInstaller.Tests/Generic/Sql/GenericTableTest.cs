using Rinsen.DatabaseInstaller.Generic.Sql;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Rinsen.DatabaseInstaller.Tests.Generic.Sql
{
    public class GenericTableTest
    {
        public class TestTable
        {
            public int MyIdColumn { get; set; }

            public int MyColumn { get; set; }

            public string MyValue { get; set; }

            public int MyIdColumn1 { get; set; }

            public int MyIdColumn2 { get; set; }

            public int Key1 { get; set; }

            public int Key2 { get; set; }

            public int Col1 { get; set; }

            public int Col2 { get; set; }

            public int Col3 { get; set; }
        }

        [Fact]
        public void WhenCreateTable_GetCorrespondingTableScript()
        {
            var table = new List<IDbChange>().AddNewTable<TestTable>();

            table.AddColumn(m => m.MyColumn);

            var createScript = table.GetUpScript().Single();

            Assert.Equal("CREATE TABLE TestTables\r\n(\r\nMyColumn int\r\n)", createScript);
        }

        [Fact]
        public void WhenCreateTableWithAutoIncrementAndPrimaryKey_GetCorrespondingTableScript()
        {
            var table = new List<IDbChange>().AddNewTable<TestTable>();

            table.AddColumn(m => m.MyIdColumn).AutoIncrement().PrimaryKey();
            table.AddColumn(m => m.MyValue ,100);

            var createScript = table.GetUpScript().Single();

            Assert.Equal("CREATE TABLE TestTables\r\n(\r\nMyIdColumn int IDENTITY(1,1) PRIMARY KEY,\r\nMyValue nvarchar(100)\r\n)", createScript);
        }

        [Fact]
        public void WhenCreateTableWithNotNullAndPrimaryKey_GetCorrespondingTableScript()
        {
            var table = new List<IDbChange>().AddNewTable<TestTable>();

            table.AddColumn(m => m.MyIdColumn).PrimaryKey();
            table.AddColumn(m => m.MyValue, 100);

            var createScript = table.GetUpScript().Single();

            Assert.Equal("CREATE TABLE TestTables\r\n(\r\nMyIdColumn int NOT NULL PRIMARY KEY,\r\nMyValue nvarchar(100)\r\n)", createScript);
        }

        [Fact]
        public void WhenCreateTableWithNotNullAndTwoColumnsAsPrimaryKey_GetCorrespondingTableScript()
        {
            var table = new List<IDbChange>().AddNewTable<TestTable>();
            table.AddColumn(m => m.MyIdColumn1).PrimaryKey();
            table.AddColumn(m => m.MyIdColumn2).PrimaryKey();
            table.AddColumn(m => m.MyValue, 100);

            var createScript = table.GetUpScript().Single();

            Assert.Equal("CREATE TABLE TestTables\r\n(\r\nMyIdColumn1 int NOT NULL,\r\nMyIdColumn2 int NOT NULL,\r\nMyValue nvarchar(100),\r\nCONSTRAINT PK_TestTables PRIMARY KEY (MyIdColumn2,MyIdColumn1)\r\n)", createScript);
        }

        [Fact]
        public void WhenCreateTableWithNamedPrimaryKey_GetCorrespondingTableScript()
        {
            var table = new List<IDbChange>().AddNewTable<TestTable>();
            table.AddColumn(m => m.Key1).PrimaryKey("PrimaryKeyForTestTables");
            table.AddColumn(m => m.Key2).PrimaryKey("PrimaryKeyForTestTables");

            var createScript = table.GetUpScript().Single();

            Assert.Equal("CREATE TABLE TestTables\r\n(\r\nKey1 int NOT NULL,\r\nKey2 int NOT NULL,\r\nCONSTRAINT PrimaryKeyForTestTables PRIMARY KEY (Key1,Key2)\r\n)", createScript);
        }

        [Fact]
        public void WhenCreateTableWithNamedUniquesAndOneUniqueColumn_GetCorrespondingTableScript()
        {
            var table = new List<IDbChange>().AddNewTable<TestTable>();
            table.AddColumn(m => m.Col1).Unique("PrimaryKeyForTestTables");
            table.AddColumn(m => m.Col2).Unique("PrimaryKeyForTestTables");
            table.AddColumn(m => m.Col3).Unique();

            var createScript = table.GetUpScript().Single();

            Assert.Equal("CREATE TABLE TestTables\r\n(\r\nCol1 int,\r\nCol2 int,\r\nCol3 int UNIQUE,\r\nCONSTRAINT PrimaryKeyForTestTables UNIQUE (Col1,Col2)\r\n)", createScript);
        }

        [Fact]
        public void WhenColumnWithNotNull_GetCorrespondingTableScript()
        {
            var table = new List<IDbChange>().AddNewTable<TestTable>();

            table.AddColumn(m => m.MyColumn).NotNull();

            var createScript = table.GetUpScript().Single();

            Assert.Equal("CREATE TABLE TestTables\r\n(\r\nMyColumn int NOT NULL\r\n)", createScript);
        }

    }
}
