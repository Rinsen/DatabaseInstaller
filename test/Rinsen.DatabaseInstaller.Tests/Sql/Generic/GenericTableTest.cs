using System;
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

        public class NullableIntTestTable
        {
            public int? MyNullableInt { get; set; }
        }

        public class GuidTestTable
        {
            public Guid MyUniqueIdentifier { get; set; }
        }

        public class NonClusteredPrimaryKey
        {
            public int Id { get; set; }

            public Guid KeyId { get; set; }

        }

        public class NullableData
        {
            public int Id { get; set; }

            public Guid? NullableGuid { get; set; }

            public DateTime? NullableDateTime { get; set; }

            public DateTimeOffset? NullableDateTimeOffset { get; set; }

            public double? NullableDouble { get; set; }

            public decimal? NullableDecimal { get; set; }

            public byte? NullableByte { get; set; }

            public byte?[] NullableByteArray { get; set; }

            public long? NullableLong { get; set; }

            public short? NullableShort { get; set; }

            public int? NullableInt { get; set; }

            public bool? NullableBool { get; set; }

            public bool NotNullableBool { get; set; }
        }

        public class ComplexNonClusteredPrimaryKey
        {
            public int ClusteredId { get; set; }

            public Guid Id { get; set; }

            public Guid OtherId { get; set; }

        }

        public class EnumTestTable
        {
            public Setting MySetting { get; set; }

        }

        public enum Setting
        {
            First,
            Second
        }

        [Fact]
        public void WhenCreateTable_GetCorrespondingTableScript()
        {
            var table = new List<IDbChange>().AddNewTable<TestTable>();

            table.AddColumn(m => m.MyColumn);

            var createScript = table.GetUpScript().Single();

            Assert.Equal("CREATE TABLE [TestTables]\r\n(\r\n[MyColumn] int NOT NULL\r\n)", createScript);
        }

        [Fact]
        public void WhenCreateTableWithAutoIncrementAndPrimaryKey_GetCorrespondingTableScript()
        {
            var table = new List<IDbChange>().AddNewTable<TestTable>();

            table.AddColumn(m => m.MyIdColumn).AutoIncrement().PrimaryKey();
            table.AddColumn(m => m.MyValue ,100);

            var createScript = table.GetUpScript().Single();

            Assert.Equal("CREATE TABLE [TestTables]\r\n(\r\n[MyIdColumn] int IDENTITY(1,1) PRIMARY KEY,\r\n[MyValue] nvarchar(100) NOT NULL\r\n)", createScript);
        }

        [Fact]
        public void WhenCreateTableWithNotNullAndPrimaryKey_GetCorrespondingTableScript()
        {
            var table = new List<IDbChange>().AddNewTable<TestTable>();

            table.AddColumn(m => m.MyIdColumn).PrimaryKey();
            table.AddColumn(m => m.MyValue, 100);

            var createScript = table.GetUpScript().Single();

            Assert.Equal("CREATE TABLE [TestTables]\r\n(\r\n[MyIdColumn] int NOT NULL PRIMARY KEY,\r\n[MyValue] nvarchar(100) NOT NULL\r\n)", createScript);
        }

        [Fact]
        public void WhenCreateTableWithNotNullAndTwoColumnsAsPrimaryKey_GetCorrespondingTableScript()
        {
            var table = new List<IDbChange>().AddNewTable<TestTable>();
            table.AddColumn(m => m.MyIdColumn1).PrimaryKey();
            table.AddColumn(m => m.MyIdColumn2).PrimaryKey();
            table.AddColumn(m => m.MyValue, 100);

            var createScript = table.GetUpScript().Single();

            Assert.Equal("CREATE TABLE [TestTables]\r\n(\r\n[MyIdColumn1] int NOT NULL,\r\n[MyIdColumn2] int NOT NULL,\r\n[MyValue] nvarchar(100) NOT NULL,\r\nCONSTRAINT PK_TestTables PRIMARY KEY (MyIdColumn2,MyIdColumn1)\r\n)", createScript);
        }

        [Fact]
        public void WhenCreateTableWithNamedPrimaryKey_GetCorrespondingTableScript()
        {
            var table = new List<IDbChange>().AddNewTable<TestTable>();
            table.AddColumn(m => m.Key1).PrimaryKey("PrimaryKeyForTestTables");
            table.AddColumn(m => m.Key2).PrimaryKey("PrimaryKeyForTestTables");

            var createScript = table.GetUpScript().Single();

            Assert.Equal("CREATE TABLE [TestTables]\r\n(\r\n[Key1] int NOT NULL,\r\n[Key2] int NOT NULL,\r\nCONSTRAINT PrimaryKeyForTestTables PRIMARY KEY (Key1,Key2)\r\n)", createScript);
        }

        [Fact]
        public void WhenCreateTableWithNamedUniquesAndOneUniqueColumn_GetCorrespondingTableScript()
        {
            var table = new List<IDbChange>().AddNewTable<TestTable>();
            table.AddColumn(m => m.Col1).Unique("PrimaryKeyForTestTables");
            table.AddColumn(m => m.Col2).Unique("PrimaryKeyForTestTables");
            table.AddColumn(m => m.Col3).Unique();

            var createScript = table.GetUpScript().Single();

            Assert.Equal("CREATE TABLE [TestTables]\r\n(\r\n[Col1] int NOT NULL,\r\n[Col2] int NOT NULL,\r\n[Col3] int NOT NULL UNIQUE,\r\nCONSTRAINT PrimaryKeyForTestTables UNIQUE (Col1,Col2)\r\n)", createScript);
        }

        [Fact]
        public void WhenColumnWithNotNull_GetCorrespondingTableScript()
        {
            var table = new List<IDbChange>().AddNewTable<TestTable>();

            table.AddColumn(m => m.MyColumn).NotNull();

            var createScript = table.GetUpScript().Single();

            Assert.Equal("CREATE TABLE [TestTables]\r\n(\r\n[MyColumn] int NOT NULL\r\n)", createScript);
        }

        [Fact]
        public void WhenNullableInt_GetCorrespondingTableScript()
        {
            var table = new List<IDbChange>().AddNewTable<NullableIntTestTable>();

            table.AddColumn(m => m.MyNullableInt);

            var createScript = table.GetUpScript().Single();

            Assert.Equal("CREATE TABLE [NullableIntTestTables]\r\n(\r\n[MyNullableInt] int NULL\r\n)", createScript);
        }

        [Fact]
        public void WhenGuid_GetCorrespondingTableScript()
        {
            var table = new List<IDbChange>().AddNewTable<GuidTestTable>();

            table.AddColumn(m => m.MyUniqueIdentifier);

            var createScript = table.GetUpScript().Single();

            Assert.Equal("CREATE TABLE [GuidTestTables]\r\n(\r\n[MyUniqueIdentifier] uniqueidentifier NOT NULL\r\n)", createScript);
        }

        [Fact]
        public void WhenTwoUniqueIds_OneAutoIncrementClusteredColumnAndOneGuidPrimaryKey_GetCorrespondingTableScript()
        {
            var table = new List<IDbChange>().AddNewTable<NonClusteredPrimaryKey>().SetPrimaryKeyNonClustered();

            table.AddAutoIncrementColumn(m => m.Id, primaryKey: false).Unique();
            table.AddColumn(m => m.KeyId).PrimaryKey();

            var createScript = table.GetUpScript().Single();

            Assert.Equal("CREATE TABLE [NonClusteredPrimaryKeys]\r\n(\r\n[Id] int UNIQUE IDENTITY(1,1),\r\n[KeyId] uniqueidentifier NOT NULL PRIMARY KEY NONCLUSTERED\r\n)", createScript);
        }

        [Fact]
        public void WithAComplexPrimaryKeyWithTwoGuids_OneAutoIncrementClusteredIndexColumn_GetCorrespondingTableScript()
        {
            var table = new List<IDbChange>().AddNewTable<ComplexNonClusteredPrimaryKey>().SetPrimaryKeyNonClustered();

            table.AddAutoIncrementColumn(m => m.ClusteredId, primaryKey: false).Unique();
            table.AddColumn(m => m.Id).PrimaryKey();
            table.AddColumn(m => m.OtherId).PrimaryKey();

            var createScript = table.GetUpScript().Single();

            Assert.Equal("CREATE TABLE [ComplexNonClusteredPrimaryKeys]\r\n(\r\n[ClusteredId] int UNIQUE IDENTITY(1,1),\r\n[Id] uniqueidentifier NOT NULL,\r\n[OtherId] uniqueidentifier NOT NULL,\r\nCONSTRAINT PK_ComplexNonClusteredPrimaryKeys PRIMARY KEY NONCLUSTERED (OtherId,Id)\r\n)", createScript);
        }

        [Fact]
        public void WhenClusteredColumn_IsOtherThanPrimaryKey__GetCorrespondingTableScript()
        {
            var table = new List<IDbChange>().AddNewTable<NonClusteredPrimaryKey>().SetPrimaryKeyNonClustered();

            table.AddAutoIncrementColumn(m => m.Id, primaryKey: false).Unique().Clustered();
            table.AddColumn(m => m.KeyId).PrimaryKey();

            var createScript = table.GetUpScript().Single();

            Assert.Equal("CREATE TABLE [NonClusteredPrimaryKeys]\r\n(\r\n[Id] int UNIQUE CLUSTERED IDENTITY(1,1),\r\n[KeyId] uniqueidentifier NOT NULL PRIMARY KEY NONCLUSTERED\r\n)", createScript);
        }

        [Fact]
        public void WhenNullableTypes_IsMapped_GetNullableColumnsTableScript()
        {
            var table = new List<IDbChange>().AddNewTable<NullableData>().SetPrimaryKeyNonClustered();

            table.AddAutoIncrementColumn(m => m.Id);
            table.AddColumn(m => m.NotNullableBool);
            table.AddColumn(m => m.NullableBool);
            table.AddColumn(m => m.NullableByte);
            table.AddColumn(m => m.NullableByteArray);
            table.AddColumn(m => m.NullableDateTime);
            table.AddColumn(m => m.NullableDateTimeOffset);
            table.AddColumn(m => m.NullableDecimal);
            table.AddColumn(m => m.NullableDouble);
            table.AddColumn(m => m.NullableGuid);
            table.AddColumn(m => m.NullableInt);
            table.AddColumn(m => m.NullableLong);
            table.AddColumn(m => m.NullableShort);

            var createScript = table.GetUpScript().Single();

            Assert.Equal("CREATE TABLE [NullableDatas]\r\n(\r\n[Id] int IDENTITY(1,1) PRIMARY KEY NONCLUSTERED,\r\n[NotNullableBool] bit NOT NULL,\r\n[NullableBool] bit NULL,\r\n[NullableByte] tinyint NULL,\r\n[NullableByteArray] varbinary(max) NULL,\r\n[NullableDateTime] datetime2 NULL,\r\n[NullableDateTimeOffset] datetimeoffset NULL,\r\n[NullableDecimal] decimal(18,2) NULL,\r\n[NullableDouble] float(53) NULL,\r\n[NullableGuid] uniqueidentifier NULL,\r\n[NullableInt] int NULL,\r\n[NullableLong] bigint NULL,\r\n[NullableShort] smallint NULL\r\n)", createScript);
        }

        [Fact]
        public void WhenEnumType_GetCorrespondingTableScript()
        {
            var table = new List<IDbChange>().AddNewTable<EnumTestTable>();

            table.AddColumn(m => m.MySetting);

            var createScript = table.GetUpScript().Single();

            Assert.Equal("CREATE TABLE [EnumTestTables]\r\n(\r\n[MySetting] tinyint NOT NULL\r\n)", createScript);
        }


    }
}
