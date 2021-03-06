﻿using System;
using System.Linq;
using Rinsen.DatabaseInstaller;
using Xunit;

namespace Rinsen.DatabaseInstaller.Tests.Generic.Sql
{
    public class GenericTableAlterationTests
    {

        class MyData
        {
            public string MyNewColumn { get; set; }

            public int MyOtherNewColumn { get; set; }
        }

        class Item
        {
            public Guid Id { get; set; }

        }

        [Fact]
        public void AddOneColumnToTable_GetCorrectUpScript()
        {
            // Arrange

            // Act
            var tableAlteration = new TableAlteration<MyData>("MyTable");
            tableAlteration.AddColumn(m => m.MyNewColumn, 100);
            var script = tableAlteration.GetUpScript(TestHelper.GetInstallerOptions()).Single();

            // Assert
            Assert.Equal($"ALTER TABLE [TestDb].[dbo].[MyTable] ADD{Environment.NewLine}MyNewColumn nvarchar(100) NOT NULL{Environment.NewLine}", script);

        }

        [Fact]
        public void AddOneColumnWithAllowNullToTable_GetCorrectUpScript()
        {
            // Arrange

            // Act
            var tableAlteration = new TableAlteration<MyData>("MyTable");
            tableAlteration.AddColumn(m => m.MyNewColumn, 100).Null();
            var script = tableAlteration.GetUpScript(TestHelper.GetInstallerOptions()).Single();

            // Assert
            Assert.Equal($"ALTER TABLE [TestDb].[dbo].[MyTable] ADD{Environment.NewLine}MyNewColumn nvarchar(100) NULL{Environment.NewLine}", script);
            
        }

        [Fact]
        public void AddTwoColumnsToTable_GetCorrectUpScript()
        {
            // Arrange

            // Act
            var tableAlteration = new TableAlteration<MyData>("MyTable");
            tableAlteration.AddColumn(m => m.MyNewColumn, 100);
            tableAlteration.AddColumn(m => m.MyOtherNewColumn);
            var script = tableAlteration.GetUpScript(TestHelper.GetInstallerOptions()).Single();

            // Assert
            Assert.Equal($"ALTER TABLE [TestDb].[dbo].[MyTable] ADD{Environment.NewLine}MyNewColumn nvarchar(100) NOT NULL,{Environment.NewLine}MyOtherNewColumn int NOT NULL{Environment.NewLine}", script);

        }

        [Fact]
        public void ChangeColumnFromNullToNotNull_GetCorrectUpScript()
        {
            // Arrange

            // Act
            var tableAlteration = new TableAlteration<MyData>("MyTable");
            tableAlteration.AlterColumn(m => m.MyNewColumn, 100);
            var script = tableAlteration.GetUpScript(TestHelper.GetInstallerOptions()).Single();

            // Assert
            Assert.Equal($"ALTER TABLE [TestDb].[dbo].[MyTable]{Environment.NewLine}ALTER COLUMN [MyNewColumn] nvarchar(100) NOT NULL{Environment.NewLine}", script);

        }


        //[Fact]
        //public void AlterColumnAndAddUniqueConstraint_GetCorrectUpScript()
        //{
        //    var columnAlteration = new TableAlteration<Item>("Items");
        //    columnAlteration.AlterColumn(m => m.Id).NotNull().Unique("UX_Items_Id");

        //    var script = columnAlteration.GetUpScript(TestHelper.GetInstallerOptions()).Single();

        //    Assert.Equal("ALTER TABLE [TestDb].[dbo].[Items] ALTER\r\nCOLUMN [Id] uniqueidentifier NOT NULL\r\nADD CONSTRAINT UX_Items_Id UNIQUE (Id)\r\n", script);
        //}
        // 
    }
}
