﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Rinsen.DatabaseInstaller.Tests.Sql
{
    public class AddValueTests
    {
        [Fact]
        public void AddValueToNullColumnToBeAbleToAddNotNullConstraint()
        {
            var addValue = new AddValue("MyTable");

            addValue.GuidColumn("ColumnName");

            var script = addValue.GetUpScript(TestHelper.GetInstallerOptions()).Single();

            Assert.Equal($"UPDATE [TestDb].[dbo].[MyTable]{Environment.NewLine}SET ColumnName = NEWID(){Environment.NewLine}WHERE ColumnName is NULL", script);
        }
    }
}
