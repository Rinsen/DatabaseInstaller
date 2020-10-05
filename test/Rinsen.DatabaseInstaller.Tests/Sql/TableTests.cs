using Rinsen.DatabaseInstaller.SqlTypes;
using System;
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

            var createScript = table.GetUpScript(TestHelper.GetInstallerOptions()).Single();

           Assert.Equal($"CREATE TABLE [TestDb].[dbo].[TestTables]{Environment.NewLine}({Environment.NewLine}[MyColumn] int NOT NULL{Environment.NewLine})", createScript);
        }

        [Fact]
        public void WhenCreateTableWithAutoIncrementAndPrimaryKey_GetCorrespondingTableScript()
        {
            var table = new Table("TestTables");
            table.AddColumn("MyIdColumn", new Int()).AutoIncrement().PrimaryKey();
            table.AddColumn("MyValue", new NVarChar(100));

            var createScript = table.GetUpScript(TestHelper.GetInstallerOptions()).Single();

           Assert.Equal($"CREATE TABLE [TestDb].[dbo].[TestTables]{Environment.NewLine}({Environment.NewLine}[MyIdColumn] int IDENTITY(1,1) PRIMARY KEY,{Environment.NewLine}[MyValue] nvarchar(100) NOT NULL{Environment.NewLine})", createScript);
        }

        [Fact]
        public void WhenCreateTableWithNotNullAndPrimaryKey_GetCorrespondingTableScript()
        {
            var table = new Table("TestTables");
            table.AddColumn("MyIdColumn", new Int()).PrimaryKey();
            table.AddColumn("MyValue", new NVarChar(100));

            var createScript = table.GetUpScript(TestHelper.GetInstallerOptions()).Single();

           Assert.Equal($"CREATE TABLE [TestDb].[dbo].[TestTables]{Environment.NewLine}({Environment.NewLine}[MyIdColumn] int NOT NULL PRIMARY KEY,{Environment.NewLine}[MyValue] nvarchar(100) NOT NULL{Environment.NewLine})", createScript);
        }

        [Fact]
        public void WhenCreateTableWithNotNullAndTwoColumnsAsPrimaryKey_GetCorrespondingTableScript()
        {
            var table = new Table("TestTables");
            table.AddColumn("MyIdColumn1", new Int()).PrimaryKey();
            table.AddColumn("MyIdColumn2", new Int()).PrimaryKey();
            table.AddColumn("MyValue", new NVarChar(100));

            var createScript = table.GetUpScript(TestHelper.GetInstallerOptions()).Single();

           Assert.Equal($"CREATE TABLE [TestDb].[dbo].[TestTables]{Environment.NewLine}({Environment.NewLine}[MyIdColumn1] int NOT NULL,{Environment.NewLine}[MyIdColumn2] int NOT NULL,{Environment.NewLine}[MyValue] nvarchar(100) NOT NULL,{Environment.NewLine}CONSTRAINT PK_TestTables PRIMARY KEY (MyIdColumn2,MyIdColumn1){Environment.NewLine})", createScript);
        }

        [Fact]
        public void WhenCreateTableWithNamedPrimaryKey_GetCorrespondingTableScript()
        {
            var table = new Table("TestTables");
            table.AddColumn("Key1", new Int()).PrimaryKey("PrimaryKeyForTestTables");
            table.AddColumn("Key2", new Int()).PrimaryKey("PrimaryKeyForTestTables");

            var createScript = table.GetUpScript(TestHelper.GetInstallerOptions()).Single();

           Assert.Equal($"CREATE TABLE [TestDb].[dbo].[TestTables]{Environment.NewLine}({Environment.NewLine}[Key1] int NOT NULL,{Environment.NewLine}[Key2] int NOT NULL,{Environment.NewLine}CONSTRAINT PrimaryKeyForTestTables PRIMARY KEY (Key1,Key2){Environment.NewLine})", createScript);
        }

        [Fact]
        public void WhenCreateTableWithNamedUniquesAndOneUniqueColumn_GetCorrespondingTableScript()
        {
            var table = new Table("TestTables");
            table.AddColumn("Col1", new Int()).Unique("UniqueForTestTables");
            table.AddColumn("Col2", new Int()).Unique("UniqueForTestTables");
            table.AddColumn("Col3", new Int()).Unique();

            var createScript = table.GetUpScript(TestHelper.GetInstallerOptions()).Single();

           Assert.Equal($"CREATE TABLE [TestDb].[dbo].[TestTables]{Environment.NewLine}({Environment.NewLine}[Col1] int NOT NULL,{Environment.NewLine}[Col2] int NOT NULL,{Environment.NewLine}[Col3] int NOT NULL UNIQUE,{Environment.NewLine}CONSTRAINT UniqueForTestTables UNIQUE (Col1,Col2){Environment.NewLine})", createScript);
        }

        [Fact]
        public void WhenCreateTableWithTwoNamedUniquesOnTwoUniqueColumns_GetCorrespondingTableScript()
        {
            var table = new Table("TestTables");
            table.AddColumn("Col1", new Int()).Unique("UX_TestTables_Col1");
            table.AddColumn("Col2", new Int()).Unique("UX_TestTables_Col2");
            table.AddColumn("Col3", new Int()).Unique();

            var createScript = table.GetUpScript(TestHelper.GetInstallerOptions()).Single();

           Assert.Equal($"CREATE TABLE [TestDb].[dbo].[TestTables]{Environment.NewLine}({Environment.NewLine}[Col1] int NOT NULL,{Environment.NewLine}[Col2] int NOT NULL,{Environment.NewLine}[Col3] int NOT NULL UNIQUE,{Environment.NewLine}CONSTRAINT UX_TestTables_Col1 UNIQUE (Col1),{Environment.NewLine}CONSTRAINT UX_TestTables_Col2 UNIQUE (Col2){Environment.NewLine})", createScript);
        }

        [Fact]
        public void WhenCreateTableWithNamedPrimaryKeyAndNamedUniques_GetCorrespondingTableScript()
        {
            var table = new Table("TestTables");
            table.AddColumn("Key1", new Int()).PrimaryKey("PrimaryKeyForTestTables");
            table.AddColumn("Key2", new Int()).PrimaryKey("PrimaryKeyForTestTables");
            table.AddColumn("Col1", new Int()).Unique("UniqueForTestTables");
            table.AddColumn("Col2", new Int()).Unique("UniqueForTestTables");
            table.AddColumn("Col3", new Int()).Unique();

            var createScript = table.GetUpScript(TestHelper.GetInstallerOptions()).Single();

           Assert.Equal($"CREATE TABLE [TestDb].[dbo].[TestTables]{Environment.NewLine}({Environment.NewLine}[Key1] int NOT NULL,{Environment.NewLine}[Key2] int NOT NULL,{Environment.NewLine}[Col1] int NOT NULL,{Environment.NewLine}[Col2] int NOT NULL,{Environment.NewLine}[Col3] int NOT NULL UNIQUE,{Environment.NewLine}CONSTRAINT UniqueForTestTables UNIQUE (Col1,Col2),{Environment.NewLine}CONSTRAINT PrimaryKeyForTestTables PRIMARY KEY (Key1,Key2){Environment.NewLine})", createScript);
        }

        [Fact]
        public void WhenColumnWithNotNull_GetCorrespondingTableScript()
        {
            var table = new Table("TestTables");
            table.AddColumn("MyColumn", new Int()).NotNull();

            var createScript = table.GetUpScript(TestHelper.GetInstallerOptions()).Single();

           Assert.Equal($"CREATE TABLE [TestDb].[dbo].[TestTables]{Environment.NewLine}({Environment.NewLine}[MyColumn] int NOT NULL{Environment.NewLine})", createScript);
        }

        [Fact]
        public void WhenGuidColumn_GetCorrespondingTableScript()
        {
            var table = new Table("TestTables");
            table.AddColumn("MyColumn", new SqlTypes.Guid()).NotNull();

            var createScript = table.GetUpScript(TestHelper.GetInstallerOptions()).Single();

           Assert.Equal($"CREATE TABLE [TestDb].[dbo].[TestTables]{Environment.NewLine}({Environment.NewLine}[MyColumn] uniqueidentifier NOT NULL{Environment.NewLine})", createScript);
        }

        [Fact]
        public void WhenCreateTableWithAutoIncrementAsNoPrimaryKey_GetCorrespondingTableScript()
        {
            var table = new Table("TestTables");
            table.AddColumn("MyIdColumn", new Int()).AutoIncrement(primaryKey: false);
            table.AddColumn("MyValue", new NVarChar(100));

            var createScript = table.GetUpScript(TestHelper.GetInstallerOptions()).Single();

           Assert.Equal($"CREATE TABLE [TestDb].[dbo].[TestTables]{Environment.NewLine}({Environment.NewLine}[MyIdColumn] int IDENTITY(1,1),{Environment.NewLine}[MyValue] nvarchar(100) NOT NULL{Environment.NewLine})", createScript);
        }

        [Fact]
        public void WhenNamedUniqueColumn_GetCorrespondingTableScript()
        {
            var table = new Table("TestTables");
            table.AddColumn("MyColumn", new SqlTypes.Guid()).Unique("UX_TestTables_MyColumn");
             
            var createScript = table.GetUpScript(TestHelper.GetInstallerOptions()).Single();

           Assert.Equal($"CREATE TABLE [TestDb].[dbo].[TestTables]{Environment.NewLine}({Environment.NewLine}[MyColumn] uniqueidentifier NOT NULL,{Environment.NewLine}CONSTRAINT UX_TestTables_MyColumn UNIQUE (MyColumn){Environment.NewLine})", createScript);
        }


    }
}
